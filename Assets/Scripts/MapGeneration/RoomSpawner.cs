using Assets.Scripts.MapGeneration;
using Assets.Scripts.Pools;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using static Unity.Burst.Intrinsics.X86.Avx;
using Random = UnityEngine.Random;

public class RoomSpawner : MonoBehaviour 
{
	[Header("Directions")]
	public int openingDirection;
	// 1 --> need bottom door --> top exit
	// 2 --> need top door --> bottom exit
	// 3 --> need left door --> right exit
	// 4 --> need right door --> left exit
	[SerializeField] private int newEntryChance = 1;

	[Header("References")]
	//private RoomPool roomPool;
	private RoomTemplates templates;
	private GameObject room = null;

	[Header("Spawning")]
	private int rand;
	public bool spawned = false;

	public float waitTime = 4f;

	void Start(){
		//must destroy instead of setting inactive as the rooms will continue to spawn on top of each other even when set inactive
		Destroy(gameObject, waitTime);

		templates = RoomTemplates.Instance;

		//templates = GameManager.Instance.templates;
		newEntryChance = templates.newEntryChance;

		//Invoke(nameof(Spawn), 0.01f); //0.01f means that 100 rooms should be spawned every second - so room generation is much faster
		Invoke(nameof(Spawn), openingDirection / 100f);
	}

	//private IEnumerator Delay()
	//{
	//	yield return new WaitForSeconds(waitTime);
	//	gameObject.SetActive(false);
	//}

	//spawning the next room
	void Spawn(){
		if(spawned == false){
			//GameObject currentRoom = transform.parent.parent.gameObject;

			if(openingDirection == 1){
				// Need to spawn a room with a BOTTOM door.
				rand = Random.Range(0, templates.bottomRooms.Length);
				//room = RoomPool.Instance.GetPooledRoom(templates.bottomRooms[rand].name);
				room = RoomTemplates.Instance.bottomRooms[rand];
				//room = Instantiate(templates.bottomRooms[rand]);

			} else if(openingDirection == 2){
				// Need to spawn a room with a TOP door.
				rand = Random.Range(0, templates.topRooms.Length);
				//room = RoomPool.Instance.GetPooledRoom(templates.topRooms[rand].name);
				room = RoomTemplates.Instance.topRooms[rand];
				//room = Instantiate(templates.topRooms[rand]);

			} else if(openingDirection == 3){
				// Need to spawn a room with a LEFT door.
				rand = Random.Range(0, templates.leftRooms.Length);
				//room = RoomPool.Instance.GetPooledRoom(templates.leftRooms[rand].name);
				room = RoomTemplates.Instance.leftRooms[rand];
				//room = Instantiate(templates.leftRooms[rand]);

			} else if(openingDirection == 4){
				// Need to spawn a room with a RIGHT door.
				rand = Random.Range(0, templates.rightRooms.Length);
				//room = RoomPool.Instance.GetPooledRoom(templates.rightRooms[rand].name);
				room = RoomTemplates.Instance.rightRooms[rand];
				//room = Instantiate(templates.rightRooms[rand]);

			}

			//check that the room isnt null
			if (room != null)
			{

				//room.transform.SetPositionAndRotation(transform.position, transform.rotation);
				room = MakeExit(room);


				//have chance to replace the room with an open room which will enable the map to extend further as the current open room (UDRL) has 4 exits
				int rand = Random.Range(0, 100);
				//if (rand <= newEntryChance || (Vector2)transform.position == templates.startRooms[0] || (Vector2)transform.position == templates.startRooms[1] || (Vector2)transform.position == templates.startRooms[2] || (Vector2)transform.position == templates.startRooms[3])
				if (rand <= newEntryChance || templates.startRooms.Contains((Vector2)transform.position))
				{
					//print("open room has replaced da room");
					//room = RoomPool.Instance.GetPooledRoom(templates.openRoom.name);
					room = RoomTemplates.Instance.openRoom;
				}
				else if (templates.bossRooms.Contains((Vector2)transform.position))
				{
					//room = RoomPool.Instance.GetPooledRoom(templates.bossRoom.name);
					room = RoomTemplates.Instance.bossRoom;
				}
				//GameObject newRoom = Instantiate(room);
				//newRoom.name = newRoom.name.Replace("(Clone)", "");
				room = Instantiate(room);
				room.name = room.name.Replace("(Clone)", "");

				room.transform.SetPositionAndRotation(transform.position, transform.rotation);
				room.SetActive(true);
				List<Wall> walls = new List<Wall> { Wall.NORTH, Wall.EAST, Wall.SOUTH, Wall.WEST };
				GameManager.Instance.thisArea.roomsData.Add(new RoomData(room.transform.position, room.transform.rotation, walls, room.name, new List<GameObject>(), new List<GameObject>()));
			}

			spawned = true;
		}
	}


	private void SpawnClosedRoom()
	{
		//room = RoomPool.Instance.GetPooledRoom(templates.closedRoom.name);
		room = RoomTemplates.Instance.closedRoom;
		room = Instantiate(room);
		room.name = room.name.Replace("(Clone)", "");
		//room = Instantiate(templates.closedRoom);
		//GameObject newRoom = Instantiate(room);
		//newRoom.name = room.name;
		room.transform.SetPositionAndRotation(transform.position, transform.rotation);

		room.SetActive(true);
		List<Wall> walls = new List<Wall> { Wall.NORTH, Wall.EAST, Wall.SOUTH, Wall.WEST };
		GameManager.Instance.thisArea.roomsData.Add(new RoomData(transform.position, Quaternion.identity, walls, room.name, new List<GameObject>(), new List<GameObject>()));
		Destroy(gameObject);
	}

	private bool CheckIfCanBeExit(GameObject room)
	{
		if ((room.name == "U" || room.name == "D" || room.name == "L" || room.name == "R") && (RoomTemplates.Instance.moveToScenes.Count > 0))
		{
			print("can be an exit");
			return true;
		}

		return false;
	}

	private GameObject MakeExit(GameObject room)
	{
		//check if its an edge room
		var adjRooms = templates.GetAdjacentRooms(transform.position);
		if (templates.CountEmptyRooms(adjRooms) <= 4)
		{
			return room;
		}

		//check if theres availability for any more exits to be added to the scene
		//if (GameManager.Instance.thisArea.numExits <= 0)
		//{
		//	return room;
		//}

		//check if room can be changed into an exit
		if (CheckIfCanBeExit(room))
		{
			GameObject newRoom = null;
			if (room.name.Equals("U"))
			{
				//newRoom = Instantiate(templates.exitRooms.);
				//newRoom = RoomPool.Instance.GetPooledRoom("UExit");
				newRoom = RoomTemplates.Instance.GetExitRoom("UExit");
			}
			else if (room.name.Equals("D"))
			{
				//newRoom = RoomPool.Instance.GetPooledRoom("DExit");
				newRoom = RoomTemplates.Instance.GetExitRoom("DExit");
			}
			else if (room.name.Equals("L"))
			{
				//newRoom = RoomPool.Instance.GetPooledRoom("LExit");
				newRoom = RoomTemplates.Instance.GetExitRoom("LExit");
			}
			else if (room.name.Equals("R"))
			{
				//newRoom = RoomPool.Instance.GetPooledRoom("RExit");
				newRoom = RoomTemplates.Instance.GetExitRoom("RExit");
			}

			newRoom.transform.SetPositionAndRotation(room.transform.position, room.transform.rotation);
			//newRoom.transform.Find("Portal").GetComponent<AddScene>().scene = SceneManager.GetSceneByBuildIndex(Random.Range(0, SceneManager.sceneCountInBuildSettings - 1)).name;

			return newRoom;
		}

		return room;
	}

	//private GameObject ReturnExitRoom(string name)
	//{
	//	foreach (GameObject room in templates.exitRooms)
	//	{
	//		if (room.name.Equals(name))
	//		{
	//			return room;
	//		}
	//	}

	//	print("NO ROOM FOUND");
	//	return null;
	//}

	void OnTriggerEnter2D(Collider2D other){
		//occurs when 2 rooms attempt to spawn a room at the same area
		if(other.CompareTag("SpawnPoint")){
			try
			{
				if (other.GetComponent<RoomSpawner>().spawned == false && spawned == false)
				{
					Invoke(nameof(SpawnClosedRoom), openingDirection / 100f);
					//Invoke(nameof(SpawnClosedRoom), 0.01f);
				}
				spawned = true;
			}
			catch{}
		}
	}
}
