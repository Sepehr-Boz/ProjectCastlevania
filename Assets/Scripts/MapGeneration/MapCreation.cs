using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MapCreation : MonoBehaviour
{
	[Header("Other scripts")]
	private RoomTemplates templates;
	//private ExtensionMethods extensions;


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
		templates = GetComponent<RoomTemplates>();
		//extensions = GetComponent<ExtensionMethods>();


		List<RoomData> roomsData = GameManager.Instance.thisArea.roomsData;

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
				List<Wall> walls = new() { Wall.NORTH, Wall.EAST, Wall.SOUTH, Wall.WEST };
				GameManager.Instance.thisArea.roomsData.Add(new RoomData(startRoom.transform.position, startRoom.transform.rotation, walls, startRoom.name, new List<GameObject>(), new List<GameObject>()));
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
			InvokeRepeating(nameof(SpawnRoomFromRoomData), 0.1f, 0.01f);
		}
	}

	#region map generation methods

	private void SpawnRoomFromRoomData()
	{
		//GameObject tmp = RoomPool.Instance.GetPooledRoom(GameManager.Instance.thisArea.roomsData[i].name);
		//print("Spawning rooms - SpawnRoomFromRoomData");

		//print(GameManager.Instance.thisArea.roomsData[i].name + "SpawnRoomFromRoomData");
		GameObject tmp = templates.GetRoom(GameManager.Instance.thisArea.roomsData[i].name);
		//print("tmp gotten - SpawnRoomFromRoomData");

		tmp = Instantiate(tmp);

		//print("tmp instantiated - SpawnRoomFromRoomData");

		tmp.name = tmp.name.Replace("(Clone)", "");

		//print("tmp renamed - SpawnRoomFromRoomData");
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

		//print("tmp position set - SpawnRoomFromRoomData");
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

			Destroy(room);

		}

		print("WALLS COPIED");

		//copy move to scenes so that when getting scenes again on generation then the scenes can be gotten again
		moveToScenes = scenesCopy;
		//clear rooms list
		GameManager.Instance.thisArea.rooms.Clear();
		//spawn rooms from rooms data
		InvokeRepeating(nameof(SpawnRoomFromRoomData), 0.1f, 0.01f);
	}


	private IEnumerator IsMapFinished()
	{
		while (true)
		{
			int start = GameManager.Instance.thisArea.roomsData.Count;
			yield return new WaitForSeconds(0.5f);
			int end = GameManager.Instance.thisArea.roomsData.Count;

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
