using Assets.Scripts.MapGeneration;
using Assets.Scripts.Pools;
using System.Collections;
using System.Collections.Generic;
using System.Net;
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


	[Header("Room data")]
	private List<GameObject> rooms;
	private List<RoomData> roomsData;

	private int i = 0; //used when spawning rooms from roomsdata
	private Vector2[] directions =
	{
		new Vector2(-10, 10),
		new Vector2(0, 10),
		new Vector2(10, 10),
		new Vector2(10, 0),
		new Vector2(10, -10),
		new Vector2(0, -10),
		new Vector2(-10, -10),
		new Vector2(-10, 0),
	};

	[SerializeField]private int horizontalChance = 3;
	[SerializeField]private int verticalChance = 5;
	[SerializeField]private int enlargeChance = 7;



	private void Start()
	{
		//roomData = GameManager.Instance.roomData;

		//InvokeRepeating("CheckRoomLengths", 0f, 0.05f);

		//check if rooms are empty
		if (AreRoomsEmpty())
		{
			//if empty then generate a new map
			print("rooms are empty");

			roomsData = GameManager.Instance.thisArea.roomsData;

			//getting and spawning start rooms onto the map
			GameObject tmp;
			foreach (RoomData data in roomsData)
			{
				//get pooled object
				tmp = RoomPool.Instance.GetPooledRoom(data.name);
				//keep spawn points active
				//tmp.transform.Find("SpawnPoints").gameObject.SetActive(true);
				//foreach (Transform child in tmp.transform.Find("SpawnPoints"))
				//{
				//	child.GetComponent<RoomSpawner>().spawned = false;
				//}
				//move room to the correct position
				tmp.transform.SetPositionAndRotation(data.position, data.rotation);
				//set the room active
				tmp.SetActive(true);
			}


			//for (int i = 0; i < 4; i++)
			//{
			//	//set the start rooms active if the rooms data are empty
			//	rooms[i].transform.Find("SpawnPoints").gameObject.SetActive(true);
			//	foreach (Transform child in rooms[i].transform.Find("SpawnPoints"))
			//	{
			//		child.gameObject.SetActive(true);
			//	}
			//	//GameManager.Instance.thisArea.rooms[i].SetActive(true);
			//}
		}
		else
		{
			print("rooms arent empty");

			//if the rooms arent empty then set all spawnpoints inactive so rooms dont keep spawning
			//List<GameObject> rooms = GameManager.Instance.thisArea.rooms;
			//int n = GameManager.Instance.thisArea.roomsData.Count;
			List<RoomData> roomsData = GameManager.Instance.thisArea.roomsData;
			//print(n);

			InvokeRepeating(nameof(SpawnRoomFromRoomData), 0.1f, 0.1f);

			//foreach (RoomData data in roomsData)
			//{
			//	print(data.name);
			//	tmp = RoomPool.Instance.GetPooledRoom(data.name);
			//	//set the SpawnPoints parent false so that the points stop spawning rooms
			//	tmp.transform.Find("SpawnPoints").gameObject.SetActive(false);
			//	tmp.transform.SetPositionAndRotation(data.position, data.rotation);

			//	//when rooms are set active they are added to rooms because of the code in AddRoom.cs Start()
			//	//GameManager.Instance.thisArea.rooms.Add(tmp);

			//	tmp.SetActive(true);
			//	//GameManager.Instance.thisArea.rooms.Add(tmp);
			//}

			//for (int i = 0; i < n; i++)
			//{
			//	//get a pooled object with the same name
			//	tmp = RoomPool.Instance.GetPooledObject(null, roomsData[i].name);
			//	//tmp.transform.Find("SpawnPoints").gameObject.SetActive(false);
			//	//Destroy(tmp.transform.Find("SpawnPoints").gameObject);
			//	//tmp.transform.Find("SpawnPoints").gameObject.SetActive(false);
			//	print("a");
			//	tmp.transform.SetPositionAndRotation(roomsData[i].position, roomsData[i].rotation);
			//	print("b");
			//	//tmp.transform.position = roomsData[i].position;
			//	//tmp.transform.rotation = roomsData[i].rotation;

			//	tmp.SetActive(true);
			//	print("e");
			//	GameManager.Instance.thisArea.rooms.Add(tmp);
			//	print("f");


			//	//set the spawn points of the room inactive
			//	//rooms[i].transform.Find("SpawnPoints").gameObject.SetActive(false);
			//	//foreach (Transform child in rooms[i].transform.Find("SpawnPoints"))
			//	//{
			//	//	Destroy(child.gameObject);
			//	//}
			//	////move the room to the position in roomsdata
			//	//GameManager.Instance.thisArea.rooms[i].transform.position = GameManager.Instance.thisArea.roomsData[i].position;
			//	//GameManager.Instance.thisArea.rooms[i].transform.rotation = GameManager.Instance.thisArea.roomsData[i].rotation;
			//	////rooms[i].transform.position = roomsData[i].position;
			//	////rooms[i].transform.rotation = roomsData[i].rotation;

			//	////spawn enemies and objects into room IF ROOM ISNT AN ENTRY ONE
			//}

			////foreach (GameObject room in GameManager.Instance.thisArea.rooms)
			////{
			////	room.transform.Find("SpawnPoints").gameObject.SetActive(false);
			////}
		}


		Invoke(nameof(ExtendRooms), 10f);
		Invoke(nameof(SpawnBosses), 30f);
	}

	private void SpawnRoomFromRoomData()
	{
		GameObject tmp = RoomPool.Instance.GetPooledRoom(GameManager.Instance.thisArea.roomsData[i].name);
		//set the SpawnPoints parent false so that the points stop spawning rooms
		tmp.transform.Find("SpawnPoints").gameObject.SetActive(false);
		tmp.transform.SetPositionAndRotation(GameManager.Instance.thisArea.roomsData[i].position, GameManager.Instance.thisArea.roomsData[i].rotation);

		//when rooms are set active they are added to rooms because of the code in AddRoom.cs Start()
		//GameManager.Instance.thisArea.rooms.Add(tmp);

		tmp.SetActive(true);

		i++;
	}



	private void OnApplicationQuit()
	{
		GameManager.Instance.thisArea.rooms.Clear();
		//GameManager.Instance.thisArea.roomsData.Clear(); //for testing

	}

	//check if room data in the area isnt empty
	private bool AreRoomsEmpty()
	{
		return GameManager.Instance.thisArea.roomsData.Count < 5;
		//returns true if empty, false if not empty
	}

	#region extending rooms

	//function that loops through rooms and extends them
	private void ExtendRooms()
	{
		rooms = GameManager.Instance.thisArea.rooms;
		int rand = Random.Range(0, 10);
		List<GameObject> adjacentRooms;

		foreach (GameObject room in rooms)
		{
			adjacentRooms = GetAdjacentRooms(room);

			if (rand < horizontalChance)
			{
				HorizontalExtend(room, adjacentRooms);
			}
			else if (horizontalChance < rand && rand < verticalChance)
			{
				VerticalExtend(room, adjacentRooms);
			}
			else if (verticalChance < rand && rand < enlargeChance)
			{
				EnlargeRoom(room, adjacentRooms);
			}
			else
			{
				continue;
			}


			//switch (rand)
			//{
			//	case 0:
			//		//extend a - Long horizontal room
			//		HorizontalExtend(room, adjacentRooms);
			//		break;
			//	case 1:
			//		//extend a
			//		HorizontalExtend(room, adjacentRooms);
			//		break;
			//	case 2:
			//		//extend a
			//		HorizontalExtend(room, adjacentRooms);
			//		break;
			//	case 3:
			//		//extend b - long vertical room
			//		VerticalExtend(room, adjacentRooms);
			//		break;
			//	case 4:
			//		//extend b
			//		VerticalExtend(room, adjacentRooms);
			//		break;
			//	case 5:
			//		//extend c - big square room
			//		EnlargeRoom(room, adjacentRooms);
			//		break;
			//	case 6:
			//		//extend c
			//		EnlargeRoom(room, adjacentRooms);
			//		break;
			//	case 7:
			//		//no extend
			//		break;
			//	case 8:
			//		//no extend
			//		break;
			//	case 9:
			//		//no extend
			//		break;
			//	case 10:
			//		//no extend
			//		break;
			//}
		}
	}

	private List<GameObject> GetAdjacentRooms(GameObject currentRoom)
	{
		//check if currentroom has already been extended and if it has then return
		if (currentRoom.GetComponent<AddRoom>().extended)
		{
			return null;
		}
		rooms = new List<GameObject>(); //max number of adjascent rooms is 8
		//check for any rooms above, below, to the right, left, and diagonally of the current room
		Vector2 newPos;
		GameObject room = null;
		for (int i = 0; i < 8; i++)
		{
			newPos = (Vector2)currentRoom.transform.position + directions[i];
			try
			{
				room = Physics2D.OverlapCircle(newPos, 1f).transform.parent.gameObject;
			}
			catch
			{
				print("no room present at position");
			}

			if (!room.GetComponent<AddRoom>().extended)
			{
				rooms.Add(room);
				room.GetComponent<AddRoom>().extended = true;
			}
		}
		//to see if there are any active rooms, check for any FOCUS gameobject and if there is then get the parent room
		//check in the AddRoom component for the variable extended, if its false add to rooms, otherwise dont as it will have already been extended
		//for every room returned set the extended value in addroom to true so theyre not extended again



		return rooms;
	}

	private void HorizontalExtend(GameObject currentRoom, List<GameObject> connectedRooms)
	{
		//need rooms east and west
		//find if any rooms are horizontal to the currentroom
		//pass the valid rooms to the correct DisableWalls function
	}
	private void VerticalExtend(GameObject currentRoom, List<GameObject> connectedRooms)
	{
		//need rooms north and south
	}
	private void EnlargeRoom(GameObject currentRoom, List<GameObject> connectedRooms)
	{
		//need rooms in every direction including diagonally
	}

	private void DisableNorthWalls(GameObject a, GameObject b)
	{
		//disable the north walls between the 2 rooms
		//remove the walls enum from each room
	}
	private void DisableEastWalls(GameObject a, GameObject b)
	{

	}
	private void DisableSouthWalls(GameObject a, GameObject b)
	{

	}
	private void DisableWestWalls(GameObject a, GameObject b)
	{

	}

	#endregion

	#region depreceated
	//depreceated as the room lengths are checked in room spawner so the list lengths never exceed the maxroomlength
	//private void CheckRoomLengths()
	//{
	//	if (roomData.roomsA.Count > maxRoomLength + 1)
	//	{
	//		roomData.roomsA[maxRoomLength].SetActive(false);
	//		//Destroy(roomData.roomsA[maxRoomLength]);
	//		roomData.roomsA.RemoveAt(maxRoomLength);
	//		return;
	//	}
	//	if (roomData.roomsB.Count > maxRoomLength + 1)
	//	{
	//		roomData.roomsB[maxRoomLength].SetActive(false);
	//		//Destroy(roomData.roomsB[maxRoomLength]);
	//		roomData.roomsB.RemoveAt(maxRoomLength);
	//		return;
	//	}
	//	if (roomData.roomsC.Count > maxRoomLength + 1)
	//	{
	//		roomData.roomsC[maxRoomLength].SetActive(false);
	//		//Destroy(roomData.roomsC[maxRoomLength]);
	//		roomData.roomsC.RemoveAt(maxRoomLength);
	//		return;
	//	}
	//	if (roomData.roomsD.Count > maxRoomLength + 1)
	//	{
	//		roomData.roomsD[maxRoomLength].SetActive(false);
	//		//Destroy(roomData.roomsD[maxRoomLength]);
	//		roomData.roomsD.RemoveAt(maxRoomLength);
	//		return;
	//	}
	//	if (roomData.roomsA.Count < maxRoomLength || roomData.roomsB.Count < maxRoomLength || roomData.roomsC.Count < maxRoomLength || roomData.roomsD.Count < maxRoomLength)
	//	{
	//		return;
	//	}

	//	CancelInvoke("CheckRoomLengths");

	//}
	#endregion


	//should be called when the rooms have been generated
	public void SpawnBosses()
	{

		////rand should be between half roomsA length not max room length as the length of the rooms does not always reach max room lengths so may be shorter
		//rand = Random.Range(Mathf.RoundToInt(roomData.roomsA.Count / 2), roomData.roomsA.Count);
		//roomBoss = Instantiate(boss, roomData.roomsA[rand].transform.position, Quaternion.identity);
		//roomData.roomsA.Add(roomBoss);

		//rand = Random.Range(Mathf.RoundToInt(roomData.roomsB.Count / 2), roomData.roomsB.Count);
		//roomBoss = Instantiate(boss, roomData.roomsB[rand].transform.position, Quaternion.identity);
		//roomData.roomsB.Add(roomBoss);

		//rand = Random.Range(Mathf.RoundToInt(roomData.roomsC.Count / 2), roomData.roomsC.Count);
		//roomBoss = Instantiate(boss, roomData.roomsC[rand].transform.position, Quaternion.identity);
		//roomData.roomsC.Add(roomBoss);

		//rand = Random.Range(Mathf.RoundToInt(roomData.roomsD.Count / 2), roomData.roomsD.Count);
		//roomBoss = Instantiate(boss, roomData.roomsD[rand].transform.position, Quaternion.identity);
		//roomData.roomsD.Add(roomBoss);


		for (int i = 0; i < numBosses; i++)
		{
			int rand = Random.Range(Mathf.RoundToInt(rooms.Count / 2), rooms.Count);
			GameObject areaBoss = Instantiate(boss, rooms[rand].transform.position, Quaternion.identity);
			roomsData[rand].AddEnemy(areaBoss);
		}

	}
}
