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

	public GameObject[] allRooms; //used when spawning rooms from rooms data

	public GameObject start;

	[Header("Map Details")]
	public List<string> moveToScenes; //queue structure, portals will fetch from here, roomspawner will check if can spawn exit by checking length of this
	//private List<GameObject> rooms;
	//private List<RoomData> roomsData;
	public Vector2[] startRooms = new Vector2[4];
	public Vector2[] bossRooms = new Vector2[2];

	private int i = 0; //used when spawning rooms from roomsdata
	private readonly Vector2[] directions =
	{
		new Vector2(-10, 10),new Vector2(0, 10),new Vector2(10, 10),
		new Vector2(-10, 0),new Vector2(0, 0),new Vector2(10, 0),
		new Vector2(-10, -10),new Vector2(0, -10),new Vector2(10, -10),
	};
	public UnityEvent<Dictionary<string, GameObject>> extendFunction = new();

	[Range(0, 100)]
	public int newEntryChance = 1;


	//make room templates a singleton as it is the most commonly and referenced object in every scene
	#region singleton
	private static RoomTemplates _instance;

	public static RoomTemplates Instance
	{
		get
		{
			return _instance;
		}
	}

	private void Awake()
	{
		_instance = this;
	}
	#endregion



	private void Start()
	{
		#region adding extension method
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
			extendFunction.AddListener(ThreexThreeRoom);
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
			extendFunction.AddListener(TwoxTwoRoom);
		}
		else if (GameManager.Instance.thisArea.area == Area.TESTING)
		{
			//test case
			//extendFunction.AddListener(HorizontalExtend);
			//extendFunction.AddListener(TwoxTwoRoom);
		}

		//print("The number of methods is -RoomTemplates : " + extendFunction.ge);
		#endregion

		List<RoomData> roomsData = GameManager.Instance.thisArea.roomsData;

		//check if rooms are empty
		if (AreRoomsEmpty())
		{
			//if empty then generate a new map
			print("rooms are empty");

			//add the first room/ entry room to room data and set it active
			List<Wall> walls = new() { Wall.NORTH, Wall.EAST, Wall.SOUTH, Wall.WEST };
			GameManager.Instance.thisArea.roomsData.Add(new RoomData(start.transform.position, start.transform.rotation, walls, start.name, new List<GameObject>(), new List<GameObject>()));
			start.SetActive(true);

			//copy the walls of the map - 10f is a decent estimate for how long it should take maximum for the map to generate
			Invoke(nameof(CopyWallsData), 10f);
		}
		else
		{
			print("rooms arent empty");

			//if the rooms arent empty then set all spawnpoints inactive so rooms dont keep spawning
			InvokeRepeating(nameof(SpawnRoomFromRoomData), 0.1f, 0.01f);
		}
	}

	#region map generation methods

	private void SpawnRoomFromRoomData()
	{
		//GameObject tmp = RoomPool.Instance.GetPooledRoom(GameManager.Instance.thisArea.roomsData[i].name);
		print("Spawning rooms - SpawnRoomFromRoomData");

		print(GameManager.Instance.thisArea.roomsData[i].name + "SpawnRoomFromRoomData");
		GameObject tmp = GetRoom(GameManager.Instance.thisArea.roomsData[i].name);
		print("tmp gotten - SpawnRoomFromRoomData");

		tmp = Instantiate(tmp);

		print("tmp instantiated - SpawnRoomFromRoomData");

		tmp.name = tmp.name.Replace("(Clone)", "");

		print("tmp renamed - SpawnRoomFromRoomData");
		//set the SpawnPoints parent false so that the points stop spawning rooms
		foreach (Transform point in tmp.transform.Find("SpawnPoints"))
		{
			if (point.name == "CENTRE")
			{
				continue;
			}

			//point.gameObject.SetActive(false);
			Destroy(point.gameObject);
		}

		tmp.transform.SetPositionAndRotation(GameManager.Instance.thisArea.roomsData[i].position, GameManager.Instance.thisArea.roomsData[i].rotation);

		print("tmp position set - SpawnRoomFromRoomData");
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
		tmp.SetActive(true);

		i++;

		//cancel the repeating invoke of this function when all rooms have been spawned
		if (i >= GameManager.Instance.thisArea.roomsData.Count)
		{
			CancelInvoke(nameof(SpawnRoomFromRoomData));
		}
	}


	//check if room data in the area isnt empty
	private bool AreRoomsEmpty()
	{
		return GameManager.Instance.thisArea.roomsData.Count < 5;
		//returns true if empty, false if not empty
	}

	private void CopyWallsData()
	{
		List<GameObject> rooms = GameManager.Instance.thisArea.rooms;

		for (int i = 0; i < rooms.Count; i++)
		{
			//get the current room
			GameObject room = rooms[i];

			//get the inactive walls
			foreach (Transform wall in room.transform.Find("Walls").transform)
			{
				if (!wall.gameObject.activeInHierarchy)
				{
					GameManager.Instance.thisArea.roomsData[i].RemoveInactiveWall(wall.name);
				}
			}
			//remove the inactive walls from the roomsdata enum at the i index

		}

		print("WALLS COPIED");
	}

	#endregion

	#region extending rooms

	public Dictionary<string, GameObject> GetAdjacentRooms(Vector2 currentPos)
	{
		//check if currentroom has already been extended and if it has then return
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
			newPos = currentPos + directions[i];
			try
			{
				room = Physics2D.OverlapCircle(newPos, 1f).transform.root.gameObject;

				if (room != null && !room.name.Contains("C"))
				{
					rooms[rooms.ElementAt(i).Key] = room;
					//room.GetComponent<AddRoom>().extended = true;
				}
			}
			catch
			{
				print("no room found - GetAdjacentRooms");
			}
			if (room == null)
			{
				continue;
			}
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

		print("The number of empty adjacent rooms is " + count);
		return count;
	}

	#region extension methods

	public void HorizontalExtend(Dictionary<string, GameObject> connectedRooms)
	{
		//current room is middle index
		//need rooms east and west
		try
		{connectedRooms["BOTTOMLEFT"].GetComponent<AddRoom>().extended = false;}
		catch {}
		try
		{connectedRooms["BOTTOM"].GetComponent<AddRoom>().extended = false;}
		catch {}
		try
		{connectedRooms["BOTTOMRIGHT"].GetComponent<AddRoom>().extended = false;}
		catch {}

		GameObject[] rooms = new GameObject[6] { connectedRooms["TOPLEFT"], connectedRooms["TOP"], connectedRooms["TOPRIGHT"], connectedRooms["LEFT"], connectedRooms["CENTRE"], connectedRooms["RIGHT"] };

		DisableHorizontalWalls(rooms[0], rooms[1]);
		DisableHorizontalWalls(rooms[1], rooms[2]);

		//find if any rooms are horizontal to the currentroom
		//pass the valid rooms to the correct DisableWalls function
	}
	public void VerticalExtend(Dictionary<string, GameObject> connectedRooms)
	{
		try
		{ connectedRooms["TOPRIGHT"].GetComponent<AddRoom>().extended = false; }
		catch { }
		try
		{ connectedRooms["RIGHT"].GetComponent<AddRoom>().extended = false; }
		catch { }
		try
		{ connectedRooms["BOTTOMRIGHT"].GetComponent<AddRoom>().extended = false; }
		catch { }

		//need rooms north and south
		GameObject[] rooms = new GameObject[3] {connectedRooms["TOP"], connectedRooms["CENTRE"], connectedRooms["BOTTOM"] };

		DisableVerticalWalls(rooms[0], rooms[1]);
		DisableVerticalWalls(rooms[1], rooms[2]);
	}

	public void TwoxTwoRoom(Dictionary<string, GameObject> connectedRooms)
	{
		//connected rooms in top left, top, left, centre

		//disable rooms horizontally
		DisableHorizontalWalls(connectedRooms["TOPLEFT"], connectedRooms["TOP"]);
		DisableHorizontalWalls(connectedRooms["LEFT"], connectedRooms["CENTRE"]);

		//disable rooms vertically
		DisableVerticalWalls(connectedRooms["TOPLEFT"], connectedRooms["LEFT"]);
		DisableVerticalWalls(connectedRooms["TOP"], connectedRooms["CENTRE"]);
	}


	public void ThreexThreeRoom(Dictionary<string, GameObject> connectedRooms)
	{
		//need rooms in every direction including diagonally
		//disable horizontal walls
		//row 1
		DisableHorizontalWalls(connectedRooms["TOPLEFT"], connectedRooms["TOP"]);
		DisableHorizontalWalls(connectedRooms["TOP"], connectedRooms["TOPRIGHT"]);
		//row 2
		DisableHorizontalWalls(connectedRooms["LEFT"], connectedRooms["CENTRE"]);
		DisableHorizontalWalls(connectedRooms["CENTRE"], connectedRooms["RIGHT"]);
		//row 3
		DisableHorizontalWalls(connectedRooms["BOTTOMLEFT"], connectedRooms["BOTTOM"]);
		DisableHorizontalWalls(connectedRooms["BOTTOM"], connectedRooms["BOTTOMRIGHT"]);


		//disable vertical walls
		//column 1
		DisableVerticalWalls(connectedRooms["TOPLEFT"], connectedRooms["LEFT"]);
		DisableVerticalWalls(connectedRooms["LEFT"], connectedRooms["BOTTOMLEFT"]);
		//column 2
		DisableVerticalWalls(connectedRooms["TOP"], connectedRooms["CENTRE"]);
		DisableVerticalWalls(connectedRooms["CENTRE"], connectedRooms["BOTTOM"]);
		//column 3
		DisableVerticalWalls(connectedRooms["TOPRIGHT"], connectedRooms["RIGHT"]);
		DisableVerticalWalls(connectedRooms["RIGHT"], connectedRooms["BOTTOMRIGHT"]);
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

		a.GetComponent<AddRoom>().extended = true;
		b.GetComponent<AddRoom>().extended = true;
	}
	private void DisableHorizontalWalls(GameObject a, GameObject b)
	{
		if (a == null || b == null)
		{
			return;
		}

		//set the walls inactive
		a.transform.Find("Walls").Find("East").gameObject.SetActive(false);
		b.transform.Find("Walls").Find("West").gameObject.SetActive(false);

		//set the extended variable to true
		a.GetComponent<AddRoom>().extended = true;
		b.GetComponent<AddRoom>().extended = true;
	}

	#endregion

	#endregion


	#region getting rooms

	//substitute for room pool so room pool can be removed from the scenes and room templates can be used instead as rooms will be instantiated instead of pooled
	public GameObject GetRoom(string roomName = null)
	{
		//loop through all rooms and return the prefab that is needed
		foreach (GameObject room in allRooms)
		{
			if (room.name.Equals(roomName))
			{
				print("room found - GetRoom");
				return room;
			}
		}

		//if nothing is passed then return null
		return null;
	}

	public GameObject GetExitRoom(string exitName = null)
	{
		foreach (GameObject room in exitRooms)
		{
			if (room.name.Equals(exitName))
			{
				return room;
			}
		}

		return null;
	}

	#endregion
}
