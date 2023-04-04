using Assets.Scripts.MapGeneration;
using Assets.Scripts.Pools;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
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
	private RoomPool roomPool;
	private RoomTemplates templates;
	private GameObject room = null;

	[Header("Spawning")]
	private int rand;
	public bool spawned = false;

	public float waitTime = 4f;

	void Start(){
		//must destroy instead of setting inactive as the rooms will continue to spawn on top of each other even when set inactive
		Destroy(gameObject, waitTime);

		templates = GameManager.Instance.templates;
		newEntryChance = templates.newEntryChance;

		Invoke(nameof(Spawn), openingDirection / 10f);
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
				room = RoomPool.Instance.GetPooledRoom(templates.bottomRooms[rand].name);
				//room = Instantiate(templates.bottomRooms[rand]);

			} else if(openingDirection == 2){
				// Need to spawn a room with a TOP door.
				rand = Random.Range(0, templates.topRooms.Length);
				room = RoomPool.Instance.GetPooledRoom(templates.topRooms[rand].name);
				//room = Instantiate(templates.topRooms[rand]);

			} else if(openingDirection == 3){
				// Need to spawn a room with a LEFT door.
				rand = Random.Range(0, templates.leftRooms.Length);
				room = RoomPool.Instance.GetPooledRoom(templates.leftRooms[rand].name);
				//room = Instantiate(templates.leftRooms[rand]);

			} else if(openingDirection == 4){
				// Need to spawn a room with a RIGHT door.
				rand = Random.Range(0, templates.rightRooms.Length);
				room = RoomPool.Instance.GetPooledRoom(templates.rightRooms[rand].name);
				//room = Instantiate(templates.rightRooms[rand]);

			}

			//check that the room isnt null
			if (room != null)
			{

				room.transform.SetPositionAndRotation(transform.position, transform.rotation);
				room = MakeExit(room);


				//have chance to replace the room with an open room which will enable the map to extend further as the current open room (UDRL) has 4 exits
				int rand = Random.Range(0, 100);
				//if (rand <= newEntryChance || (Vector2)transform.position == templates.startRooms[0] || (Vector2)transform.position == templates.startRooms[1] || (Vector2)transform.position == templates.startRooms[2] || (Vector2)transform.position == templates.startRooms[3])
				if (rand <= newEntryChance || templates.startRooms.Contains((Vector2)transform.position))
				{
					//print("open room has replaced da room");
					room = RoomPool.Instance.GetPooledRoom(templates.openRoom.name);
				}
				else if (templates.bossRooms.Contains((Vector2)transform.position))
				{
					room = RoomPool.Instance.GetPooledRoom(templates.bossRoom.name);
				}

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
		room = RoomPool.Instance.GetPooledRoom(templates.closedRoom.name);
		//room = Instantiate(templates.closedRoom);

		room.transform.SetPositionAndRotation(transform.position, transform.rotation);

		room.SetActive(true);
		List<Wall> walls = new List<Wall> { Wall.NORTH, Wall.EAST, Wall.SOUTH, Wall.WEST };
		GameManager.Instance.thisArea.roomsData.Add(new RoomData(transform.position, Quaternion.identity, walls, room.name, new List<GameObject>(), new List<GameObject>()));
		Destroy(gameObject);
	}

	private bool CheckIfCanBeExit(GameObject room)
	{
		if (room.name == "U" || room.name == "D" || room.name == "L" || room.name == "R")
		{
			print("can be an exit");
			return true;
		}

		return false;
	}

	private GameObject MakeExit(GameObject room)
	{
		//check if its an edge room
		var adjRooms = templates.GetAdjacentRooms(room);
		if (templates.CountEmptyRooms(adjRooms) <= 4)
		{
			return room;
		}

		//check if theres availability for any more exits to be added to the scene
		if (GameManager.Instance.thisArea.numExits <= 0)
		{
			return room;
		}

		//check if room can be changed into an exit
		if (CheckIfCanBeExit(room))
		{
			GameObject newRoom = null;
			if (room.name.Equals("U"))
			{
				newRoom = RoomPool.Instance.GetPooledRoom("UExit");
			}
			else if (room.name.Equals("D"))
			{
				newRoom = RoomPool.Instance.GetPooledRoom("DExit");
			}
			else if (room.name.Equals("L"))
			{
				newRoom = RoomPool.Instance.GetPooledRoom("LExit");
			}
			else if (room.name.Equals("R"))
			{
				newRoom = RoomPool.Instance.GetPooledRoom("RExit");
			}

			newRoom.transform.SetPositionAndRotation(room.transform.position, room.transform.rotation);
			newRoom.transform.Find("Portal").GetComponent<AddScene>().scene = SceneManager.GetSceneByBuildIndex(Random.Range(0, SceneManager.sceneCountInBuildSettings - 1)).name;

			return newRoom;
		}

		return room;
	}

	void OnTriggerEnter2D(Collider2D other){
		//occurs when 2 rooms attempt to spawn a room at the same area
		if(other.CompareTag("SpawnPoint")){
			try
			{
				if (other.GetComponent<RoomSpawner>().spawned == false && spawned == false)
				{
					Invoke(nameof(SpawnClosedRoom), openingDirection / 10f);
				}
				spawned = true;
			}
			catch{}
		}
	}
}
