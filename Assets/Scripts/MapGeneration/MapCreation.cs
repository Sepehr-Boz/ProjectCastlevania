using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class MapCreation : MonoBehaviour
{
	[Header("Other scripts")]
	private RoomTemplates templates;
	//private ExtensionMethods extensions;


	public Transform mapParent;
	public bool isNewMap = true;


	public GameObject[] start;
	//public List<string> moveToScenes;
	public List<SceneEntry> moveToScenes;
	private List<SceneEntry> scenesCopy;

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
		//find mapParent if not added
		if (!mapParent)
		{
			mapParent = GameObject.Find("Map").transform;
		}


		templates = GetComponent<RoomTemplates>();
		//extensions = GetComponent<ExtensionMethods>();


		List<RoomData> roomsData = GameManager.Instance.thisArea.rooms;

		//check if rooms are empty
		if (AreRoomsEmpty())
		{
			isNewMap = true;

			//make a copy of the move to scenes so that on first map generation then the portals can have a list of scenes to get from
			scenesCopy = new List<SceneEntry>(moveToScenes);


			//if empty then generate a new map
			print("MAKE NEW MAP");

			//add the first room/ entry room to room data and set it active
			foreach (GameObject startRoom in start)
			{
				//dont need to add room data as the room data will be added by themselves in AddRoom


				////List<Wall> walls = new() { Wall.NORTH, Wall.EAST, Wall.SOUTH, Wall.WEST };
				//RoomData newData = new RoomData(startRoom.transform.position, startRoom.name, new() { Wall.NORTH, Wall.EAST, Wall.SOUTH, Wall.WEST });
				////GameManager.Instance.thisArea.roomsData.Add(new RoomData(startRoom.transform.position, startRoom.transform.rotation, walls, startRoom.name, new List<GameObject>(), new List<GameObject>()));
				//GameManager.Instance.thisArea.rooms.Add(newData);

				startRoom.SetActive(true);
			}

			StartCoroutine(IsMapFinished());

			//copy the walls of the map - 10f is a decent estimate for how long it should take maximum for the map to generate
			//Invoke(nameof(CopyWallsData), 10f);
		}
		else
		{
			print("SPAWN EXISTENT MAP");
			isNewMap = false;

			//if the rooms arent empty then set all spawnpoints inactive so rooms dont keep spawning
			InvokeRepeating(nameof(SpawnRoomFromRoomData), 0.1f, 0.01f);
		}
	}

	#region map generation methods

	private void SpawnRoomFromRoomData()
	{
		//GameObject tmp = RoomPool.Instance.GetPooledRoom(GameManager.Instance.thisArea.roomsData[i].name);
		//print("Spawning rooms - SpawnRoomFromRoomData");

		isNewMap = false;

		//print(GameManager.Instance.thisArea.roomsData[i].name + "SpawnRoomFromRoomData");
		GameObject tmp = templates.GetRoom(GameManager.Instance.thisArea.rooms[i].name);
		//print("tmp gotten - SpawnRoomFromRoomData");

		tmp = Instantiate(tmp);

		//add the room as a child to mapParent
		tmp.transform.parent = mapParent;

		//print("tmp instantiated - SpawnRoomFromRoomData");

		tmp.name = tmp.name.Replace("(Clone)", "");

		//print("tmp renamed - SpawnRoomFromRoomData");
		//set the SpawnPoints parent false so that the points stop spawning rooms
		//foreach (Transform point in tmp.transform.Find("SpawnPoints"))
		//{
		//	if (point.name == "CENTRE")
		//	{
		//		continue;
		//	}

		//	//point.gameObject.SetActive(false);
		//	Destroy(point.gameObject);
		//}

		//destroy all spawn points as theyre not needed anymore as the map isnt to be made anymore
		Destroy(tmp.transform.Find("SpawnPoints"));


		tmp.transform.position = GameManager.Instance.thisArea.rooms[i].position;
		//tmp.transform.SetPositionAndRotation(GameManager.Instance.thisArea.roomsData[i].position, GameManager.Instance.thisArea.roomsData[i].rotation);



		//for each wall check if its enum equivalent exists in activewalls and if so then set active otherwise destroy the wall
		List<Wall> activeWalls = GameManager.Instance.thisArea.rooms[i].activeWalls;
		//check for each enum if the active walls contains it
		tmp.transform.Find("Walls").Find("North").gameObject.SetActive(activeWalls.Contains(Wall.NORTH));//will set true if contains and false if doesnt contain
		tmp.transform.Find("Walls").Find("East").gameObject.SetActive(activeWalls.Contains(Wall.EAST));
		tmp.transform.Find("Walls").Find("South").gameObject.SetActive(activeWalls.Contains(Wall.SOUTH));
		tmp.transform.Find("Walls").Find("West").gameObject.SetActive(activeWalls.Contains(Wall.WEST));
		//^ faster method than 1) loop through walls and set them inactive then 2) loop through active walls and set the correct walls active




		//print("tmp position set - SpawnRoomFromRoomData");
		//set all walls inactive first
		//foreach (Transform wall in tmp.transform.Find("Walls"))
		//{
		//	wall.gameObject.SetActive(false);
		//}


		////set active walls active
		//List<Wall> activeWalls = GameManager.Instance.thisArea.roomsData[i].GetActiveWalls();
		//foreach (Wall activeWall in activeWalls)
		//{
		//	if (activeWall == Wall.NORTH)
		//	{
		//		tmp.transform.Find("Walls").Find("North").gameObject.SetActive(true);
		//	}
		//	else if (activeWall == Wall.EAST)
		//	{
		//		tmp.transform.Find("Walls").Find("East").gameObject.SetActive(true);
		//	}
		//	else if (activeWall == Wall.SOUTH)
		//	{
		//		tmp.transform.Find("Walls").Find("South").gameObject.SetActive(true);
		//	}
		//	else if (activeWall == Wall.WEST)
		//	{
		//		tmp.transform.Find("Walls").Find("West").gameObject.SetActive(true);
		//	}
		//	else
		//	{
		//		print("error has occurred");
		//	}
		//}

		//when rooms are set active they are added to rooms because of the code in AddRoom.cs Start()
		tmp.SetActive(true);

		i++;

		//cancel the repeating invoke of this function when all rooms have been spawned
		if (i >= GameManager.Instance.thisArea.rooms.Count)
		{
			CancelInvoke(nameof(SpawnRoomFromRoomData));
		}
	}


	//check if room data in the area isnt empty
	private bool AreRoomsEmpty()
	{
		return GameManager.Instance.thisArea.rooms.Count < 5;
		//returns true if empty, false if not empty
	}

	private void CopyWallsData()
	{
		//List<GameObject> rooms = GameManager.Instance.thisArea.rooms;

		//for (int i = 0; i < rooms.Count; i++)
		//{
		//	//get the current room
		//	GameObject room = rooms[i];

		////get the inactive walls
		//foreach (Transform wall in room.transform.Find("Walls").transform)
		//{
		//	if (!wall.gameObject.activeInHierarchy)
		//	{
		//		GameManager.Instance.thisArea.roomsData[i].RemoveInactiveWall(wall.name);
		//	}
		//}
		////remove the inactive walls from the roomsdata enum at the i index

		//	Destroy(room);

		//}

		//loop through all the rooms childed to map
		int index = 0;
		foreach (Transform child in mapParent.transform)
		{
			//get the room
			GameObject room = child.gameObject;

			//find which walls are inactive and remove them from rooms in areadata
			foreach (Transform wall in child.Find("Walls"))
			{
				if (!wall.gameObject.activeInHierarchy)
				{
					GameManager.Instance.thisArea.rooms[index].RemoveInactiveWall(wall.name);
				}
			}

			//destroy the room and increment the index to refer to the next room data
			Destroy(room);
			index++;
		}

		//print(index);

		//for (int i = 0; i < mapParent.transform.childCount; i++)
		//{
		//	GameObject room = mapParent.GetChild(i).gameObject;

		//	//get the inactive walls
		//	foreach (Transform wall in room.transform.Find("Walls").transform)
		//	{
		//		if (!wall.gameObject.activeInHierarchy)
		//		{
		//			GameManager.Instance.thisArea.rooms[i].RemoveInactiveWall(wall.name);
		//		}
		//	}
		//	//remove the inactive walls from the rooms enum at the i index

		//	//Destroy(room, 0.2f);
		//}

		//print(i);


		//foreach (Transform room in mapParent.transform)
		//{
		//	Destroy(room.gameObject);
		//}

		print("WALLS COPIED");

		//copy move to scenes so that when getting scenes again on generation then the scenes can be gotten again
		moveToScenes = scenesCopy;
		//clear rooms list
		//GameManager.Instance.thisArea.rooms.Clear();
		//spawn rooms from rooms data
		InvokeRepeating(nameof(SpawnRoomFromRoomData), 0.1f, 0.01f);
	}


	private IEnumerator IsMapFinished()
	{
		while (true)
		{
			int start = GameManager.Instance.thisArea.rooms.Count;
			yield return new WaitForSeconds(0.5f);
			int end = GameManager.Instance.thisArea.rooms.Count;

			//print("Start: " + start);
			//print("End: " + end);

			if (start == end)
			{
				//finished = true;
				//CopyWallsData();
				Invoke(nameof(CopyWallsData), 4f);
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
