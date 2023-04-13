using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Analytics;

public class MapCreation : MonoBehaviour
{
	[Header("Other scripts")]
	private RoomTemplates templates;
	//private ExtensionMethods extensions;

	public Transform mapParent;

	public GameObject[] start;
	//public List<string> moveToScenes;
	public List<SceneEntry> moveToScenes;
	private List<SceneEntry> scenesCopy;


	private RoomNode head;
	public RoomNode tail;


    private int i = 0;

	//private bool finished = false;

	//#region singleton
	//private static MapCreation _instance;

	//public static MapCreation Instance
	//{
	//	get
	//	{
	//		return _instance;
	//	}
	//}

	//private void Awake()
	//{
	//	_instance = this;
	//}
	//#endregion

	private void Start()
	{
		mapParent = GameObject.Find("Map").transform;

		templates = GetComponent<RoomTemplates>();
		//extensions = GetComponent<ExtensionMethods>();

		//List<RoomData> roomsData = GameManager.Instance.thisArea.roomsData;
		head = GameManager.Instance.thisArea.roomHead;
		tail = head;

		//check if rooms are empty
		if (AreRoomsEmpty())
		{
			//make a copy of the move to scenes so that on first map generation then the portals can have a list of scenes to get from
			scenesCopy = new List<SceneEntry>(moveToScenes);


			//if empty then generate a new map
			print("MAKE NEW MAP");

			//add the first room/ entry room to room data and set it active
			foreach (GameObject startRoom in start)
			{
				//List<Wall> walls = new() { Wall.NORTH, Wall.EAST, Wall.SOUTH, Wall.WEST };

				//make new room node and pass it as the next node and iterate to the next node
				RoomData data = new RoomData(templates.GetRoom(startRoom.name), startRoom.transform.position, new() { Wall.NORTH, Wall.EAST, Wall.SOUTH, Wall.WEST });
				RoomNode newNode = new RoomNode(startRoom, data, null);

				////first room node in area data is empty so add to the current tail and iterate to the next after so that the first node doesnt end up empty
				//tail.val = data;
				//tail.roomInstance = startRoom;

				tail.next = newNode;
				tail = tail.next;

				//add to map size counter
				GameManager.Instance.thisArea.currentMapSize++;

				//GameManager.Instance.thisArea.roomsData.Add(new RoomData(startRoom.transform.position, startRoom.transform.rotation, walls, startRoom.name, new List<GameObject>(), new List<GameObject>()));

				//set the start room active
				startRoom.SetActive(true);
			}

			StartCoroutine(IsMapFinished());

			//copy the walls of the map - 10f is a decent estimate for how long it should take maximum for the map to generate
			//Invoke(nameof(CopyWallsData), 10f);
		}
		else
		{
			print("SPAWN EXISTENT MAP");

			//if the rooms arent empty then set all spawnpoints inactive so rooms dont keep spawning
			//InvokeRepeating(nameof(SpawnRoomFromRoomData), 0.1f, 0.01f);
			Invoke(nameof(SpawnRoomFromRoomNode), 0.1f);
		}
	}

	#region map generation methods

	private void SpawnRoomFromRoomNode()
	{
		//start from head and iterate the tail until next is null
		tail = head.next;

		while (tail.next != null)
		{
			//pull the room prefab from the RoomData val.roomPrefab
			GameObject tmp = Instantiate(tail.val.roomPrefab);
			tail.roomInstance = tmp;
			tmp.SetActive(false);

			//add room to map parent
			tmp.transform.parent = mapParent;

			//get the position from RoomData val
			tmp.transform.position = tail.val.position;
			tmp.name.Replace("(Clone)", "");

			//destroy the spawn points other than CENTRE
			foreach (Transform point in tmp.transform.Find("SpawnPoints"))
			{
				if (point.name == "CENTRE")
				{
					continue;
				}

				Destroy(point.gameObject);
			}

			//set all walls inactive then set them active based on activewalls
			foreach (Transform wall in tmp.transform.Find("Walls")) { wall.gameObject.SetActive(false); };
			
			List<Wall> activeWalls = tail.val.activeWalls;
			foreach (Wall wall in activeWalls)
			{
				switch (wall)
				{
					case Wall.NORTH:
						tmp.transform.Find("Walls").Find("North").gameObject.SetActive(true);
						continue;
					case Wall.SOUTH:
						tmp.transform.Find("Walls").Find("East").gameObject.SetActive(true);
						continue;
					case Wall.EAST:
						tmp.transform.Find("Walls").Find("South").gameObject.SetActive(true);
						continue;
					case Wall.WEST:
						tmp.transform.Find("Walls").Find("West").gameObject.SetActive(true);
						continue;
					default:
						continue;
				}
			}

			tmp.SetActive(true);


			tail = tail.next;
		}
	}

	//private void SpawnRoomFromRoomData()
	//{
	//	//GameObject tmp = RoomPool.Instance.GetPooledRoom(GameManager.Instance.thisArea.roomsData[i].name);
	//	//print("Spawning rooms - SpawnRoomFromRoomData");

	//	//print(GameManager.Instance.thisArea.roomsData[i].name + "SpawnRoomFromRoomData");

	//	GameObject tmp = templates.GetRoom(GameManager.Instance.thisArea.roomsData[i].name);
	//	//print("tmp gotten - SpawnRoomFromRoomData");

	//	tmp = Instantiate(tmp);

	//	tmp.transform.parent = mapParent;

	//	//print("tmp instantiated - SpawnRoomFromRoomData");

	//	tmp.name = tmp.name.Replace("(Clone)", "");

	//	//print("tmp renamed - SpawnRoomFromRoomData");
	//	//set the SpawnPoints parent false so that the points stop spawning rooms
	//	foreach (Transform point in tmp.transform.Find("SpawnPoints"))
	//	{
	//		if (point.name == "CENTRE")
	//		{
	//			continue;
	//		}

	//		//point.gameObject.SetActive(false);
	//		Destroy(point.gameObject);
	//	}

	//	tmp.transform.SetPositionAndRotation(GameManager.Instance.thisArea.roomsData[i].position, GameManager.Instance.thisArea.roomsData[i].rotation);

	//	//print("tmp position set - SpawnRoomFromRoomData");
	//	//set all walls inactive first
	//	foreach (Transform wall in tmp.transform.Find("Walls"))
	//	{
	//		wall.gameObject.SetActive(false);
	//	}


	//	//set active walls active
	//	List<Wall> activeWalls = GameManager.Instance.thisArea.roomsData[i].GetActiveWalls();
	//	foreach (Wall activeWall in activeWalls)
	//	{
	//		if (activeWall == Wall.NORTH)
	//		{
	//			tmp.transform.Find("Walls").Find("North").gameObject.SetActive(true);
	//		}
	//		else if (activeWall == Wall.EAST)
	//		{
	//			tmp.transform.Find("Walls").Find("East").gameObject.SetActive(true);
	//		}
	//		else if (activeWall == Wall.SOUTH)
	//		{
	//			tmp.transform.Find("Walls").Find("South").gameObject.SetActive(true);
	//		}
	//		else if (activeWall == Wall.WEST)
	//		{
	//			tmp.transform.Find("Walls").Find("West").gameObject.SetActive(true);
	//		}
	//		else
	//		{
	//			print("error has occurred");
	//		}
	//	}

	//	//when rooms are set active they are added to rooms because of the code in AddRoom.cs Start()
	//	tmp.SetActive(true);

	//	i++;

	//	//cancel the repeating invoke of this function when all rooms have been spawned
	//	if (i >= GameManager.Instance.thisArea.roomsData.Count)
	//	{
	//		CancelInvoke(nameof(SpawnRoomFromRoomData));
	//	}
	//}


	//check if room data in the area isnt empty
	private bool AreRoomsEmpty()
	{
		//return GameManager.Instance.thisArea.roomsData.Count < 5;
		return head.next == null;
		//returns true if empty, false if not empty
	}

	private void CopyWallsData()
	{
		//iterate from the head to the tail
		tail = head.next;

		while (tail.next != null)
		{
			//get room instance
			GameObject room = tail.roomInstance;

			foreach (Transform wall in room.transform.Find("Walls"))
			{
				if (!wall.gameObject.activeInHierarchy)
				{
					tail.val.RemoveInactiveWall(wall.name);
				}
			}

			Destroy(room);
			tail.roomInstance = null;

			tail = tail.next;
		}

		print("WALLS COPIED");

		moveToScenes = scenesCopy;
		Invoke(nameof(SpawnRoomFromRoomNode), 0.1f);




		//List<GameObject> rooms = GameManager.Instance.thisArea.rooms;

		//for (int i = 0; i < rooms.Count; i++)
		//{
		//	//get the current room
		//	GameObject room = rooms[i];

		//	//get the inactive walls
		//	foreach (Transform wall in room.transform.Find("Walls").transform)
		//	{
		//		if (!wall.gameObject.activeInHierarchy)
		//		{
		//			GameManager.Instance.thisArea.roomsData[i].RemoveInactiveWall(wall.name);
		//		}
		//	}
		//	//remove the inactive walls from the roomsdata enum at the i index

		//	Destroy(room);

		//}

		//print("WALLS COPIED");

		////copy move to scenes so that when getting scenes again on generation then the scenes can be gotten again
		//moveToScenes = scenesCopy;
		////clear rooms list
		////GameManager.Instance.thisArea.rooms.Clear();
		////spawn rooms from rooms data
		////InvokeRepeating(nameof(SpawnRoomFromRoomData), 0.1f, 0.01f);
		//Invoke(nameof(SpawnRoomFromRoomNode), 0.1f);
	}


	private IEnumerator IsMapFinished()
	{
		while (true)
		{
			//int start = GameManager.Instance.thisArea.roomsData.Count;
			int start = GameManager.Instance.thisArea.currentMapSize;
			yield return new WaitForSeconds(1f);
			//int end = GameManager.Instance.thisArea.roomsData.Count;
			int end = GameManager.Instance.thisArea.currentMapSize;

			print("Start: " + start);
			print("End: " + end);

			if (start == end)
			{
				//finished = true;
				//CopyWallsData();
				Invoke(nameof(CopyWallsData), 3f);
				break;
			}
		}
	}

	#endregion
}


[System.Serializable]public struct SceneEntry
{
	public string sceneName; //scene name - have to be manually added
	public Vector2 newPos; //positions are where the start rooms are in the next scene - have to be manually added

	public SceneEntry(string name, Vector2 pos)
	{
		this.sceneName = name;
		this.newPos = pos;
	}
}
