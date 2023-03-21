﻿using Assets.Scripts.MapGeneration;
using Assets.Scripts.Pools;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Runtime.CompilerServices;
using Random = UnityEngine.Random;
using UnityEngine;
using static Unity.Burst.Intrinsics.X86.Avx;

public class RoomTemplates : MonoBehaviour 
{
	[Header("Room References")]
	[Space(2)]
	public GameObject[] bottomRooms;
	public GameObject[] topRooms;
	public GameObject[] leftRooms;
	public GameObject[] rightRooms;
	public GameObject closedRoom;
	public GameObject openRoom;

	public GameObject boss;

	//[SerializeField]public RoomData roomData;

	[Header("Script Variables")]
	//public float waitTime;
	public int maxRoomLength = 50; //accessed from roomspawner also
	public int numBosses = 1; //how many bosses to spawn
	public int numStartRooms = 4;


	[Header("Room data")]
	private List<GameObject> rooms;
	private List<RoomData> roomsData;

	private int i = 0; //used when spawning rooms from roomsdata
	private Vector2[] directions =
	{
		new Vector2(-10, 10),new Vector2(0, 10),new Vector2(10, 10),
		new Vector2(-10, 0),new Vector2(0, 0),new Vector2(10, 0),
		new Vector2(-10, -10),new Vector2(0, -10),new Vector2(10, -10),
	};

	[Range(0, 10)]
	[SerializeField]private int horizontalChance = 3;
	[Range(0, 10)]
	[SerializeField]private int verticalChance = 5;
	[Range(0, 10)]
	[SerializeField]private int enlargeChance = 7;



	private void Start()
	{
		List<RoomData> roomsData = GameManager.Instance.thisArea.roomsData;

		//check if rooms are empty
		if (AreRoomsEmpty())
		{
			//if empty then generate a new map
			print("rooms are empty");

			//getting and spawning start rooms onto the map
			GameObject tmp;
			foreach (RoomData data in roomsData)
			{
				//get pooled object
				tmp = RoomPool.Instance.GetPooledRoom(data.name);
				//set the spawn points active
				tmp.transform.Find("SpawnPoints").gameObject.SetActive(true);
				//move room to the correct position
				tmp.transform.SetPositionAndRotation(data.position, data.rotation);
				//set the room active
				tmp.SetActive(true);
			}

			Invoke(nameof(ExtendRooms), 5f);
			Invoke(nameof(CopyWallsData), 10f);
		}
		else
		{
			print("rooms arent empty");

			//if the rooms arent empty then set all spawnpoints inactive so rooms dont keep spawning

			InvokeRepeating(nameof(SpawnRoomFromRoomData), 0.1f, 0.05f);
		}

		Invoke(nameof(SpawnBosses), 20f);
	}

	private void SpawnRoomFromRoomData()
	{
		GameObject tmp = RoomPool.Instance.GetPooledRoom(GameManager.Instance.thisArea.roomsData[i].name);
		//set the SpawnPoints parent false so that the points stop spawning rooms
		//Destroy(tmp.transform.Find("SpawnPoints").gameObject);
		//tmp.transform.Find("SpawnPoints").gameObject.SetActive(false);
		foreach (Transform point in tmp.transform.Find("SpawnPoints"))
		{
			if (point.name == "CENTRE")
			{
				continue;
			}

			point.gameObject.SetActive(false);
		}

		tmp.transform.SetPositionAndRotation(GameManager.Instance.thisArea.roomsData[i].position, GameManager.Instance.thisArea.roomsData[i].rotation);
		//set all walls inactive first
		foreach (Transform wall in tmp.transform.Find("Walls"))
		{
			wall.gameObject.SetActive(false);
		}


		//set active walls active
		List<Wall> activeWalls = GameManager.Instance.thisArea.roomsData[i].GetActiveWalls();
		foreach (Wall activeWall in activeWalls)
		{
			if (activeWall == Wall.NORTH)
			{
				tmp.transform.Find("Walls").Find("North").gameObject.SetActive(true);
			}
			else if (activeWall == Wall.EAST)
			{
				tmp.transform.Find("Walls").Find("East").gameObject.SetActive(true);
			}
			else if (activeWall == Wall.SOUTH)
			{
				tmp.transform.Find("Walls").Find("South").gameObject.SetActive(true);
			}
			else if (activeWall == Wall.WEST)
			{
				tmp.transform.Find("Walls").Find("West").gameObject.SetActive(true);
			}
			else
			{
				print("error has occurred");
			}
		}

		//when rooms are set active they are added to rooms because of the code in AddRoom.cs Start()
		//GameManager.Instance.thisArea.rooms.Add(tmp);

		tmp.SetActive(true);

		i++;
	}

	//private void OnApplicationQuit()
	//{
	//	GameManager.Instance.thisArea.rooms.Clear();

	//	if (GameManager.Instance.thisArea.area == Area.MAZEA || GameManager.Instance.thisArea.area == Area.MAZEB)
	//	{
	//		GameManager.Instance.thisArea.roomsData.Clear();
	//		//get rid of all data so a new map generates every time for each maze
	//	}
	//	//areas other than the mazes will only generate once however
	//}

	//check if room data in the area isnt empty
	private bool AreRoomsEmpty()
	{
		return GameManager.Instance.thisArea.roomsData.Count < numStartRooms + 1;
		//returns true if empty, false if not empty
	}

	#region extending rooms

	//function that loops through rooms and extends them
	private void ExtendRooms()
	{
		print("extending rooms");
		rooms = GameManager.Instance.thisArea.rooms;
		GameObject[] adjacentRooms;

		foreach (GameObject room in rooms)
		{
			adjacentRooms = GetAdjacentRooms(room);
			if (adjacentRooms == null)
			{
				continue;
			}

			int rand = Random.Range(0, 10);

			if (rand < horizontalChance)
			{
				StartCoroutine(HorizontalExtend(adjacentRooms));
			}
			else if (horizontalChance < rand && rand < verticalChance)
			{
				StartCoroutine(VerticalExtend(adjacentRooms));
			}
			else if (verticalChance < rand && rand < enlargeChance)
			{
				StartCoroutine(EnlargeRoom(adjacentRooms));

				for (int j = 0; j < 9; j++)
				{
					if (j == 0 || j == 1 || j == 2 || j == 5 || j == 8)
					{
						adjacentRooms[j].GetComponent<AddRoom>().extended = adjacentRooms[j] != null ? true : false;
					}
				}
			}
			else
			{
				continue;
			}
		}
	}

	private void CopyWallsData()
	{
		List<GameObject> rooms = GameManager.Instance.thisArea.rooms;

		for (int i = 0; i < rooms.Count; i++)
		{
			//get the current room
			GameObject room = rooms[i];

			//get the inactive walls
			//List<string> inactiveWalls = new List<string>();
			foreach (Transform wall in room.transform.Find("Walls").transform)
			{
				if (!wall.gameObject.activeInHierarchy)
				{
					print("wall is inactive");
					GameManager.Instance.thisArea.roomsData[i].RemoveInactiveWall(wall.name);
					//inactiveWalls.Add(wall.name);
				}
			}

			//print(inactiveWalls);
			//remove the inactive walls from the roomsdata enum at the i index
			//GameManager.Instance.thisArea.roomsData[i].RemoveInactiveWalls(inactiveWalls);

		}
	}

	private GameObject[] GetAdjacentRooms(GameObject currentRoom)
	{
		//check if currentroom has already been extended and if it has then return
		if (currentRoom.GetComponent<AddRoom>().extended)
		{
			return null;
		}
		GameObject[] rooms = new GameObject[9]; //max number of adjascent rooms is 8
		//check for any rooms above, below, to the right, left, and diagonally of the current room
		Vector2 newPos;
		GameObject room = null;
		for (int i = 0; i < 9; i++)
		{
			newPos = (Vector2)currentRoom.transform.position + directions[i];
			try
			{
				room = Physics2D.OverlapCircle(newPos, 1f).transform.root.gameObject;
				print(room.name);
			}
			catch
			{
				print("no room found");
			}
			if (room == null)
			{
				rooms[i] = null;
				continue;
			}

			if (!room.GetComponent<AddRoom>().extended)
			{
				rooms[i] = room;
				room.GetComponent<AddRoom>().extended = true;
			}
		}
		//to see if there are any active rooms, check for any FOCUS gameobject and if there is then get the parent room
		//check in the AddRoom component for the variable extended, if its false add to rooms, otherwise dont as it will have already been extended
		//for every room returned set the extended value in addroom to true so theyre not extended again



		return rooms;
	}

	private IEnumerator HorizontalExtend(GameObject[] connectedRooms)
	{
		//current room is middle index
		//need rooms east and west
		GameObject[] rooms = new GameObject[3] { connectedRooms[3], connectedRooms[4], connectedRooms[5] };
		yield return new WaitForSeconds(0.1f);
		DisableHorizontalWalls(rooms[0], rooms[1]);
		yield return new WaitForSeconds(0.1f);
		DisableHorizontalWalls(rooms[1], rooms[2]);
		//find if any rooms are horizontal to the currentroom
		//pass the valid rooms to the correct DisableWalls function
	}
	private IEnumerator VerticalExtend(GameObject[] connectedRooms)
	{
		//need rooms north and south
		GameObject[] rooms = new GameObject[3] { connectedRooms[1], connectedRooms[4], connectedRooms[7] };
		yield return new WaitForSeconds(0.1f);
		DisableVerticalWalls(rooms[0], rooms[1]);
		yield return new WaitForSeconds(0.1f);
		DisableVerticalWalls(rooms[1], rooms[2]);
	}
	private IEnumerator EnlargeRoom(GameObject[] connectedRooms)
	{
		//need rooms in every direction including diagonally
		GameObject[] rooms = new GameObject[4] { connectedRooms[3], connectedRooms[4], connectedRooms[6], connectedRooms[7] };
		yield return new WaitForSeconds(0.1f);
		DisableHorizontalWalls(rooms[0], rooms[1]);
		yield return new WaitForSeconds(0.1f);
		DisableHorizontalWalls(rooms[2], rooms[3]);
		yield return new WaitForSeconds(0.1f);
		DisableVerticalWalls(rooms[0], rooms[2]);
		yield return new WaitForSeconds(0.1f);
		DisableVerticalWalls(rooms[1], rooms[3]);
		yield return new WaitForSeconds(0.1f);
	}

	private void DisableVerticalWalls(GameObject a, GameObject b)
	{
		if (a == null || b == null)
		{
			return;
		}

		//disable the north walls between the 2 rooms
		//remove the walls enum from each room
		a.transform.Find("Walls").Find("South").gameObject.SetActive(false);
		b.transform.Find("Walls").Find("North").gameObject.SetActive(false);
	}
	private void DisableHorizontalWalls(GameObject a, GameObject b)
	{
		if (a == null || b == null)
		{
			return;
		}


		a.transform.Find("Walls").Find("East").gameObject.SetActive(false);
		b.transform.Find("Walls").Find("West").gameObject.SetActive(false);


	}

	#endregion

	//should be called when the rooms have been generated
	public void SpawnBosses()
	{
		for (int i = 0; i < numBosses; i++)
		{
			int rand = Random.Range(Mathf.RoundToInt(rooms.Count / 2), rooms.Count);
			GameObject areaBoss = Instantiate(boss, rooms[rand].transform.position, Quaternion.identity);
			print(rand);
			GameManager.Instance.thisArea.roomsData[rand].AddEnemy(areaBoss);
		}

	}
}
