using Assets.Scripts.MapGeneration;
using Assets.Scripts.Pools;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Runtime.CompilerServices;
using Random = UnityEngine.Random;
using UnityEngine;
using static Unity.Burst.Intrinsics.X86.Avx;
using UnityEngine.Events;
using Unity.VisualScripting;
using System.Linq;

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
	public GameObject bossRoom;
	public GameObject[] exitRooms;
	//public GameObject boss;

	//[SerializeField]public RoomData roomData;

	[Header("Script Variables")]
	//public float waitTime;
	//public int maxRoomLength = 50; //accessed from roomspawner also
	public int numBosses = 1; //how many bosses to spawn
	public int numStartRooms = 4;


	[Header("Room data")]
	private List<GameObject> rooms;
	private List<RoomData> roomsData;
	//public Vector2[] startRooms = new List<Transform>();
	//public Transform[] startRooms = new Transform[4];
	public Vector2[] startRooms = new Vector2[4];
	public Vector2[] bossRooms = new Vector2[2];

	private int i = 0; //used when spawning rooms from roomsdata
	private readonly Vector2[] directions =
	{
		new Vector2(-10, 10),new Vector2(0, 10),new Vector2(10, 10),
		new Vector2(-10, 0),new Vector2(0, 0),new Vector2(10, 0),
		new Vector2(-10, -10),new Vector2(0, -10),new Vector2(10, -10),
	};

	//[SerializeField] private UnityEvent extend;
	public UnityEvent<Dictionary<string, GameObject>> extendFunction = new();
	//[Range(0, 10)] [SerializeField] private int extendChance; //not needed as while looping through rooms unaffected rooms are still set as extended so has a "random" chance
	//[Range(0, 10)]
	//[SerializeField] private int horizontalChance = 3;
	//[Range(0, 10)]
	//[SerializeField] private int verticalChance = 5;
	//[Range(0, 10)]
	//[SerializeField] private int enlargeChance = 7;

	[Range(0, 100)]
	public int newEntryChance = 1;


	private void Start()
	{
		if (GameManager.Instance.thisArea.area == Area.AREA1)
		{
			extendFunction.AddListener(HorizontalExtend);
		}
		else if (GameManager.Instance.thisArea.area == Area.AREA2)
		{
			extendFunction.AddListener(VerticalExtend);
		}
		else if (GameManager.Instance.thisArea.area == Area.AREA3)
		{
			extendFunction.AddListener(EnlargeRoom);
		}
		else if (GameManager.Instance.thisArea.area == Area.AREA4)
		{
			//next listener, can add multiple methods
			extendFunction.AddListener(HorizontalExtend);
			extendFunction.AddListener(VerticalExtend);
		}
		else if (GameManager.Instance.thisArea.area == Area.AREA5)
		{
			//next mlistener, maybe do nothing?
		}
		else if (GameManager.Instance.thisArea.area == Area.TESTING)
		{
			//test case
			extendFunction.AddListener(HorizontalExtend);
		}
		//extendFunction.AddListener(extend);
		//extend.AddListener(VerticalExtend);
		//extend.AddListener(ex);


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
				//tmp = Instantiate(openRoom);
				//set the spawn points active
				tmp.transform.Find("SpawnPoints").gameObject.SetActive(true);
				//move room to the correct position
				tmp.transform.SetPositionAndRotation(data.position, data.rotation);
				//set the room active
				tmp.SetActive(true);
			}

			//Invoke(nameof(ExtendRooms), 6f);
			//Invoke(nameof(ExtendClosedExits), 7f);
			//Invoke(nameof(CreateExits), 7f);

			Invoke(nameof(CopyWallsData), 10f);
		}
		else
		{
			print("rooms arent empty");

			//if the rooms arent empty then set all spawnpoints inactive so rooms dont keep spawning

			InvokeRepeating(nameof(SpawnRoomFromRoomData), 0.1f, 0.05f);
		}

		//Invoke(nameof(SpawnBosses), 10f);

		Invoke(nameof(DeleteUnusedRooms), 13f);
	}

	//private void CreateExits()
	//{
	//	//loop through all the rooms
	//	int n = GameManager.Instance.thisArea.roomsData.Count;
	//	List<GameObject> rooms = GameManager.Instance.thisArea.rooms;
	//	List<RoomData> roomsData = GameManager.Instance.thisArea.roomsData;

	//	for (int i = 0; i < n; i++)
	//	{
	//		GameObject room = rooms[i];

	//		//get number of valid adjascent rooms
	//		GameObject[] adjRooms = GetAdjacentRooms(room);
	//		int validNeighbours = 0;
	//		foreach (GameObject adj in adjRooms)
	//		{
	//			validNeighbours++;
	//		}



	//		if ("UDLR".Contains(room.name) && validNeighbours >= 5)
	//		{
	//			bool extended = false;
	//			foreach (Transform wall in room.transform.Find("Walls"))
	//			{
	//				if (!wall.gameObject.activeInHierarchy)
	//				{
	//					extended = true;
	//				}
	//			}

	//			if (!extended && GameManager.Instance.thisArea.numExits > 0)
	//			{
	//				//check if the number of areas in the scene is more than 0

	//				//replace room with portal room
	//				print("room replaced at " + i);

	//				//room = exitRoom;
	//				//GameManager.Instance.thisArea.rooms[i].
	//				//GameObject newRoom = exitRooms.Select(x.name.Contains(room.name)).FirstOrDefault();
	//				//GameObject newRoom = exitRooms.Contains(room.name);
	//				GameObject newRoom = null;
	//				if (room.name.Equals("U"))
	//				{
	//					newRoom = RoomPool.Instance.GetPooledRoom(exitRooms[0].name);
	//					//newRoom = exitRooms[0];
	//				}
	//				else if (room.name.Equals("D"))
	//				{
	//					newRoom = RoomPool.Instance.GetPooledRoom(exitRooms[1].name);
	//					//newRoom = exitRooms[1];
	//				}
	//				else if (room.name.Equals("L"))
	//				{
	//					newRoom = RoomPool.Instance.GetPooledRoom(exitRooms[2].name);
	//					//newRoom = exitRooms[2];
	//				}
	//				else if (room.name.Equals("R"))
	//				{
	//					newRoom = RoomPool.Instance.GetPooledRoom(exitRooms[3].name);
	//					//newRoom = exitRooms[3];
	//				}
	//				if (newRoom == null)
	//				{
	//					continue;
	//				}
	//				newRoom.transform.SetPositionAndRotation(room.transform.position, room.transform.rotation);
	//				newRoom.SetActive(true);

	//				//GameManager.Instance.thisArea.rooms[i] = newRoom;

	//				room.SetActive(false);

	//				//remove the replcaed room and roomdata and add the new roomdata and room to the end of the lists
	//				GameManager.Instance.thisArea.rooms.RemoveAt(i);
	//				GameManager.Instance.thisArea.roomsData.RemoveAt(i);
	//				i--;
	//				GameManager.Instance.thisArea.roomsData.Add(new RoomData(newRoom.transform.position, newRoom.transform.rotation, new List<Wall> {Wall.NORTH, Wall.EAST, Wall.SOUTH, Wall.WEST}, newRoom.name, null, null));
	//				//GameManager.Instance.thisArea.roomsData[i].Rename(newRoom.name);
	//				//replace roomdata room name with portal room
	//				////////spawn portal at the room
	//				//////add the data to the room data
	//				//decrement the numexits
	//				GameManager.Instance.thisArea.numExits--;
	//				//add the scene and position to the portal script
	//			}
	//		}
	//	}


	//	//foreach (GameObject room in GameManager.Instance.thisArea.rooms)
	//	//{
	//	//	//check if room is an edge room
	//	//	//edge room = U/D/L/R, >=5 empty neighbours, not extended(all 4 walls active)
	//	//	if ("UDLR".Contains(room.name) && GetAdjacentRooms(room).Length >= 5)
	//	//	{
	//	//		//check if all walls are active to see if its not extended
	//	//		bool extended = false;
	//	//		foreach (GameObject wall in room.transform.Find("Walls"))
	//	//		{
	//	//			if (!wall.activeInHierarchy)
	//	//			{
	//	//				extended = true;
	//	//			}
	//	//		}

	//	//		//if not extended then the room meets all the requirements
	//	//		if (!extended && GameManager.Instance.thisArea.numExits > 0)
	//	//		{
	//	//			//check if the number of areas in the scene is more than 0

	//	//			//spawn portal at the room
	//	//			//add the data to the room data
	//	//			//decrement the numexits
	//	//			//add the scene and position to the portal script
	//	//		}
	//	//	}
	//	//}
	//}

	private void DeleteUnusedRooms()
	{
		foreach (GameObject pooledRoom in RoomPool.Instance.pooledObjects)
		{
			if (!pooledRoom.activeInHierarchy)
			{
				Destroy(pooledRoom, 0.1f);
			}
		}
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

		if (i >= rooms.Count)
		{
			CancelInvoke(nameof(SpawnRoomFromRoomData));
		}
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
	//private void ExtendRooms()
	//{
	//	int n = GameManager.Instance.thisArea.roomsData.Count;

	//	for (int i = 0; i < n; i++)
	//	{
	//		//get the current rooms and adjacent rooms
	//		GameObject currentRoom = GameManager.Instance.thisArea.rooms[i];
	//		RoomData currentRoomData = GameManager.Instance.thisArea.roomsData[i];

	//		////////////////////////
	//		//GameObject[] adjRooms = GetAdjacentRooms(currentRoom);

	//		//get the top, right, bottom, left rooms names
	//		string[] adjacentRoomNames = new string[4]{null, null, null, null};
	//		if (adjRooms[1] != null)
	//		{
	//			adjacentRoomNames[0] = adjRooms[1].name;
	//			//adjacentRoomNames.Append(adjRooms[1].name);
	//		}
	//		if (adjRooms[3] != null)
	//		{
	//			adjacentRoomNames[1] = adjRooms[3].name;
	//			//adjacentRoomNames.Append(adjRooms[3].name);
	//		}
	//		if (adjRooms[5] != null)
	//		{
	//			adjacentRoomNames[2] = adjRooms[5].name;
	//			//adjacentRoomNames.Append(adjRooms[5].name);
	//		}
	//		if (adjRooms[7] != null)
	//		{
	//			adjacentRoomNames[3] = adjRooms[7].name;
	//			//adjacentRoomNames.Append(adjRooms[7].name);
	//		}

	//		//Debug.Log("names = " + String.Join("",new List<string>(adjacentRoomNames).ToArray()));
	//		//string[] adjRoomNames = new string[] { adjRooms[1].name, adjRooms[3].name, adjRooms[5].name, adjRooms[7].name };
	//		//adjRooms = new[]{ adjRooms[1], adjRooms[3], adjRooms[4], adjRooms[5], adjRooms[7] };
	//		//adjRooms = {adjRooms[1], adjRooms[3], adjRooms[4], adjRooms[5], adjRooms[7]};

	//		//make new room name
	//		string newRoomName = "";
	//		//check each room name individually as theyll have different logic ( e.g. top/bottom rooms only UD matter, left/right rooms only LR matter)
	//		if (adjacentRoomNames[0] != null && adjacentRoomNames[0].Contains("D")) //top room
	//		{
	//			//add the needed exit to the new room name
	//			newRoomName += "U";
	//		}
	//		if (adjacentRoomNames[1] != null && adjacentRoomNames[1].Contains("R")) //left room
	//		{
	//			newRoomName += "L";
	//		}
	//		if (adjacentRoomNames[2] != null && adjacentRoomNames[2].Contains("L")) //right room
	//		{
	//			newRoomName += "R";
	//		}
	//		if (adjacentRoomNames[3] != null && adjacentRoomNames[3].Contains("U")) //bottom room
	//		{
	//			newRoomName += "D";
	//		}

	//		//check if new room name is matching the current room name
	//		//if it is then dont spawn a new room as the current room is valid
	//		//if (newRoomName.Equals(currentRoom.name))
	//		//{
	//		//	continue;
	//		//}
	//		if (!newRoomName.Equals(currentRoom.name) && !newRoomName.Equals(""))
	//		{
	//			//if theyre not equal then a new room is needed to be spawned and replace the current room
	//			print("new room name is" + newRoomName + "names = " + String.Join(" ", new List<string>(adjacentRoomNames).ToArray()));
	//			//get a new room with the new room name
	//			GameObject newRoom = RoomPool.Instance.GetPooledRoom(newRoomName);
	//			//move the new room to where the current room is
	//			newRoom.transform.SetPositionAndRotation(currentRoom.transform.position, currentRoom.transform.rotation);
	//			newRoom.SetActive(true);

	//			//remove the current room and roomdata from the lists
	//			currentRoom.SetActive(false);
	//			GameManager.Instance.thisArea.rooms.RemoveAt(i);
	//			GameManager.Instance.thisArea.roomsData.RemoveAt(i);
	//			i--;

	//			//add the new room to rooms data - it will add itself to rooms
	//			List<Wall> walls = new() { Wall.NORTH, Wall.EAST, Wall.SOUTH, Wall.WEST };
	//			GameManager.Instance.thisArea.roomsData.Add(new RoomData(newRoom.transform.position, newRoom.transform.rotation, walls, newRoomName, null, null));

	//		}

	//	}




	//	//print("extending rooms");
	//	//rooms = GameManager.Instance.thisArea.rooms;
	//	//GameObject[] adjacentRooms;

	//	//foreach (GameObject room in rooms)
	//	//{
	//	//	adjacentRooms = GetAdjacentRooms(room);
	//	//	if (adjacentRooms == null)
	//	//	{
	//	//		continue;
	//	//	}
	//	//	//extend the rooms so that any blocked exits are opened
	//	//	//ExtendClosedExits(adjacentRooms);
	//	//	extendFunction.Invoke(adjacentRooms);

	//	//	//int rand = Random.Range(0, 10);

	//	//	//if (rand < extendChance)
	//	//	//{
	//	//	//	//do the extend function
	//	//	//	extendFunction.Invoke(adjacentRooms);
	//	//	//}
	//	//	//else
	//	//	//{
	//	//	//	//do nothing otherwise 
	//	//	//}

	//	//		//do the added extend method
	//	//		//extend.Invoke(adjacentRooms);
	//	//		//Invoke()
	//	//		//StartCoroutine(extendFunction(adjacentRooms));
			

	//	//	//if (rand < horizontalChance)
	//	//	//{
	//	//	//	StartCoroutine(HorizontalExtend(adjacentRooms));
	//	//	//}
	//	//	//else if (horizontalChance < rand && rand < verticalChance)
	//	//	//{
	//	//	//	StartCoroutine(VerticalExtend(adjacentRooms));
	//	//	//}
	//	//	//else if (verticalChance < rand && rand < enlargeChance)
	//	//	//{
	//	//	//	StartCoroutine(EnlargeRoom(adjacentRooms));

	//	//	//	//for (int j = 0; j < 9; j++)
	//	//	//	//{
	//	//	//	//	if (j == 0 || j == 1 || j == 2 || j == 5 || j == 8)
	//	//	//	//	{
	//	//	//	//		adjacentRooms[j].GetComponent<AddRoom>().extended = adjacentRooms[j] != null ? true : false;
	//	//	//	//	}
	//	//	//	//}
	//	//	//}
	//	//	//else
	//	//	//{
	//	//	//	continue;
	//	//	//}
	//	//}
	//}

	//private void ExtendClosedExits()
	//{
	//	//GameObject[] rooms = GetAdjacentRooms()
	//	//GameObject[] rooms = new GameObject[5] { connectedRooms[1], connectedRooms[3], connectedRooms[4], connectedRooms[5], connectedRooms[7] };
	//	foreach (GameObject room in GameManager.Instance.thisArea.rooms)
	//	{
	//		GameObject[] connectedRooms = GetAdjacentRooms(room);

	//		//check which directions are open by checking the name of the room UDLR and that the room in the direction is existent and not empty
	//		//middle/current room is the one in the centre of the array
	//		if (connectedRooms[4].name.Contains("U") && connectedRooms[1] != null)
	//		{
	//			//set the walls between the 2 rooms inactive
	//			DisableVerticalWalls(connectedRooms[1], connectedRooms[4]);
	//		}
	//		if (connectedRooms[4].name.Contains("D") && connectedRooms[7] != null)
	//		{
	//			DisableVerticalWalls(connectedRooms[4], connectedRooms[7]);
	//		}
	//		if (connectedRooms[4].name.Contains("L") && connectedRooms[3] != null)
	//		{
	//			DisableHorizontalWalls(connectedRooms[3], connectedRooms[4]);
	//		}
	//		if (connectedRooms[4].name.Contains("R") && connectedRooms[5] != null)
	//		{
	//			DisableHorizontalWalls(connectedRooms[4], connectedRooms[5]);
	//		}
	//	}
	//}

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
					//print("wall is inactive");
					GameManager.Instance.thisArea.roomsData[i].RemoveInactiveWall(wall.name);
					//inactiveWalls.Add(wall.name);
				}
			}

			//print(inactiveWalls);
			//remove the inactive walls from the roomsdata enum at the i index
			//GameManager.Instance.thisArea.roomsData[i].RemoveInactiveWalls(inactiveWalls);

		}
	}



	public Dictionary<string, GameObject> GetAdjacentRooms(GameObject currentRoom)
	{
		//check if currentroom has already been extended and if it has then return
		//if (currentRoom.GetComponent<AddRoom>().extended)
		//{
		//	return null;
		//}
		//GameObject[] rooms = new GameObject[9]; //max number of adjascent rooms is 8
		//check for any rooms above, below, to the right, left, and diagonally of the current room
		Dictionary<string, GameObject> rooms = new Dictionary<string, GameObject>(9) 
		{
			{"TOPLEFT", null },  {"TOP", null }, {"TOPRIGHT", null },
			{"LEFT", null }, {"CENTRE", null }, {"RIGHT", null },
			{"BOTTOMLEFT", null }, {"BOTTOM", null }, {"BOTTOMRIGHT", null }
		};

		Vector2 newPos;
		GameObject room = null;
		for (int i = 0; i < 9; i++)
		{
			newPos = (Vector2)currentRoom.transform.position + directions[i];
			try
			{
				room = Physics2D.OverlapCircle(newPos, 1f).transform.root.gameObject;
				//print(room.name);

				if (room != null && !room.name.Contains("C"))
				{
					rooms[rooms.ElementAt(i).Key] = room;
					room.GetComponent<AddRoom>().extended = true;
				}
			}
			catch
			{
				print("no room found - GetAdjacentRooms");
			}
			if (room == null)
			{
				//rooms.ElementAt(i).Value = null;
				//rooms[i] = null;
				continue;
			}

			//if (!room.GetComponent<AddRoom>().extended)
			//{
			//	rooms[rooms.ElementAt(i).Key] = room;
			//	//rooms[i] = room;
			//	room.GetComponent<AddRoom>().extended = true;
			//}
		}
		//to see if there are any active rooms, check for any FOCUS gameobject and if there is then get the parent room
		//check in the AddRoom component for the variable extended, if its false add to rooms, otherwise dont as it will have already been extended
		//for every room returned set the extended value in addroom to true so theyre not extended again



		return rooms;
	}

	public int CountEmptyRooms(Dictionary<string, GameObject> adjRooms)
	{
		int count = 0;
		foreach (GameObject room in adjRooms.Values)
		{
			if (room == null || room.name.Contains("C"))
			{
				count++;
			}
		}

		return count;
	}

	public void HorizontalExtend(Dictionary<string, GameObject> connectedRooms)
	{
		//current room is middle index
		//need rooms east and west
		//GameObject[] rooms = GetAdjacentRooms(currentRoom);
		//rooms = new GameObject[3] { rooms[3], rooms[4], rooms[5] };
		try
		{
			connectedRooms["BOTTOMLEFT"].GetComponent<AddRoom>().extended = false;
			connectedRooms["BOTTOM"].GetComponent<AddRoom>().extended = false;
			connectedRooms["BOTTOMRIGHT"].GetComponent<AddRoom>().extended = false;
		}
		catch{}


		//GameObject[] rooms = new GameObject[3] {connectedRooms["LEFT"], connectedRooms["CENTRE"], connectedRooms["RIGHT"]};
		////yield return new WaitForSeconds(0.1f);
		////yield return new WaitForSeconds(0.1f);
		//DisableHorizontalWalls(rooms[0], rooms[1]);
		//DisableHorizontalWalls(rooms[1], rooms[2]);

		GameObject[] rooms = new GameObject[6] { connectedRooms["TOPLEFT"], connectedRooms["TOP"], connectedRooms["TOPRIGHT"], connectedRooms["LEFT"], connectedRooms["CENTRE"], connectedRooms["RIGHT"] };

		DisableHorizontalWalls(rooms[0], rooms[1]);
		DisableHorizontalWalls(rooms[1], rooms[2]);

		DisableHorizontalWalls(rooms[3], rooms[4]);
		DisableHorizontalWalls(rooms[4], rooms[5]);

		//DisableHorizontalWalls(rooms[0], rooms[1]);
		//DisableHorizontalWalls(rooms[1], rooms[2]);
		//DisableHorizontalWalls(rooms[3], rooms[4]);
		//DisableHorizontalWalls(rooms[4], rooms[5]);

		//DisableVerticalWalls(rooms[0], rooms[3]);
		//DisableVerticalWalls(rooms[1], rooms[4]);
		//DisableVerticalWalls(rooms[2], rooms[5]);
		//find if any rooms are horizontal to the currentroom
		//pass the valid rooms to the correct DisableWalls function
	}
	public void VerticalExtend(Dictionary<string, GameObject> connectedRooms)
	{
		try
		{
			connectedRooms["TOPRIGHT"].GetComponent<AddRoom>().extended = false;
			connectedRooms["RIGHT"].GetComponent<AddRoom>().extended = false;
			connectedRooms["BOTTOMRIGHT"].GetComponent<AddRoom>().extended = false;
		}
		catch{}

		//need rooms north and south
		GameObject[] rooms = new GameObject[3] {connectedRooms["TOP"], connectedRooms["CENTRE"], connectedRooms["BOTTOM"] };
		//yield return new WaitForSeconds(0.1f);
		//yield return new WaitForSeconds(0.1f);

		DisableVerticalWalls(rooms[0], rooms[1]);
		DisableVerticalWalls(rooms[1], rooms[2]);

		//DisableVerticalWalls(rooms[0], rooms[1]);
		//DisableVerticalWalls(rooms[1], rooms[2]);
		//DisableVerticalWalls(rooms[3], rooms[4]);
		//DisableVerticalWalls(rooms[4], rooms[5]);

		//DisableHorizontalWalls(rooms[0], rooms[3]);
		//DisableHorizontalWalls(rooms[1], rooms[4]);
		//DisableHorizontalWalls(rooms[2], rooms[5]);
	}
	public void EnlargeRoom(Dictionary<string, GameObject> connectedRooms)
	{
		//need rooms in every direction including diagonally
		//////GameObject[] rooms = new GameObject[9] {};
		//disable horizontal walls
		//row 1
		//yield return new WaitForSeconds(0.1f);
		DisableHorizontalWalls(connectedRooms["TOPLEFT"], connectedRooms["TOP"]);
		//yield return new WaitForSeconds(0.1f);
		DisableHorizontalWalls(connectedRooms["TOP"], connectedRooms["TOPRIGHT"]);
		//row 2
		//yield return new WaitForSeconds(0.1f);
		DisableHorizontalWalls(connectedRooms["LEFT"], connectedRooms["CENTRE"]);
		//yield return new WaitForSeconds(0.1f);
		DisableHorizontalWalls(connectedRooms["CENTRE"], connectedRooms["RIGHT"]);
		//row 3
		//yield return new WaitForSeconds(0.1f);
		DisableHorizontalWalls(connectedRooms["BOTTOMLEFT"], connectedRooms["BOTTOM"]);
		//yield return new WaitForSeconds(0.1f);
		DisableHorizontalWalls(connectedRooms["BOTTOM"], connectedRooms["BOTTOMRIGHT"]);


		//disable vertical walls
		//column 1
		//yield return new WaitForSeconds(0.1f);
		DisableVerticalWalls(connectedRooms["TOPLEFT"], connectedRooms["LEFT"]);
		//yield return new WaitForSeconds(0.1f);
		DisableVerticalWalls(connectedRooms["LEFT"], connectedRooms["BOTTOMLEFT"]);
		//yield return new WaitForSeconds(0.1f);
		//column 2
		DisableVerticalWalls(connectedRooms["TOP"], connectedRooms["CENTRE"]);
		//yield return new WaitForSeconds(0.1f);
		DisableVerticalWalls(connectedRooms["CENTRE"], connectedRooms["BOTTOM"]);
		//yield return new WaitForSeconds(0.1f);
		//column 3
		DisableVerticalWalls(connectedRooms["TOPRIGHT"], connectedRooms["RIGHT"]);
		//yield return new WaitForSeconds(0.1f);
		DisableVerticalWalls(connectedRooms["RIGHT"], connectedRooms["BOTTOMRIGHT"]);
		//yield return new WaitForSeconds(0.1f);
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
	//public void SpawnBosses()
	//{
	//	int count = 0;
	//	GameObject areaBoss;
	//	//loop through room data
	//	for (int i = 0; i < numBosses; i++)
	//	{
	//		//find if any room data have enemies
	//		//if they do then spawn and increment local count
	//		if (GameManager.Instance.thisArea.roomsData[i].enemies.Contains(boss))
	//		{
	//			print("boss has been found");
	//			count += 1;
	//			areaBoss = Instantiate(boss, GameManager.Instance.thisArea.rooms[i].transform.position, Quaternion.identity);
	//		}
	//		continue;
	//	}

	//	while (count < numBosses)
	//	{
	//		print("new bosses being spawned");
	//		int rand = Random.Range(Mathf.RoundToInt(GameManager.Instance.thisArea.rooms.Count / 2), GameManager.Instance.thisArea.rooms.Count);
	//		areaBoss = Instantiate(boss, GameManager.Instance.thisArea.rooms[rand].transform.position, Quaternion.identity);
	//		//check if room doesnt have an exit/portal
	//		GameManager.Instance.thisArea.roomsData[rand].AddEnemy(areaBoss);
	//		print(rand);
	//		count += 1;
	//	}
	//	//if the local count is less than numBosses then spawn bosses at random indexes and add to the roomdata


	//	//for (int i = 0; i < numBosses; i++)
	//	//{
	//	//	int rand = Random.Range(Mathf.RoundToInt(rooms.Count / 2), rooms.Count);
	//	//	GameObject areaBoss = Instantiate(boss, rooms[rand].transform.position, Quaternion.identity);
	//	//	print(rand);
	//	//	GameManager.Instance.thisArea.roomsData[rand].AddEnemy(areaBoss);
	//	//}

	//}
}
