using Assets.Scripts.MapGeneration;
using Assets.Scripts.Pools;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
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
	private MapCreation mapCreation;
	private ExtensionMethods extensions;

	private GameObject room = null;

	[Header("Spawning")]
	private int rand;
	public bool spawned = false;

	public float waitTime = 4f;

	void Start(){

		//must destroy instead of setting inactive as the rooms will continue to spawn on top of each other even when set inactive
		Destroy(gameObject, waitTime);

		GameObject roomsRef = GameObject.FindGameObjectWithTag("Rooms");
		templates = roomsRef.GetComponent<RoomTemplates>();
		mapCreation = roomsRef.GetComponent<MapCreation>();
		extensions = roomsRef.GetComponent<ExtensionMethods>();

		newEntryChance = extensions.newEntryChance;

		Invoke(nameof(Spawn), openingDirection / 50f);
		//Invoke(nameof(Spawn), 0.01f);
	}

	//spawning the next room
	void Spawn(){
		if(spawned == false){
			//GameObject currentRoom = transform.parent.parent.gameObject;

			if(openingDirection == 1){
				// Need to spawn a room with a BOTTOM door.
				rand = Random.Range(0, templates.bottomRooms.Length);
				room = templates.bottomRooms[rand];
			} 
			else if(openingDirection == 2){
				// Need to spawn a room with a TOP door.
				rand = Random.Range(0, templates.topRooms.Length);
				room = templates.topRooms[rand];
			}
			else if(openingDirection == 3){
				// Need to spawn a room with a LEFT door.
				rand = Random.Range(0, templates.leftRooms.Length);
				room = templates.leftRooms[rand];
			}
			else if(openingDirection == 4){
				// Need to spawn a room with a RIGHT door.
				rand = Random.Range(0, templates.rightRooms.Length);
				room = templates.rightRooms[rand];
			}

			//check that the room isnt null
			if (room != null)
			{

				//have chance to replace the room with an open room which will enable the map to extend further as the current open room (UDRL) has 4 exits

				//keep this to make large-ish rooms but dont use it if rooms are wanted to be kept small(like 10-20ish rooms)
				//int rand = Random.Range(0, 100);
				//if (rand <= newEntryChance)
				//{
				//	room = templates.openRoom;
				//}
				room = ChangeRoom(room);
				room = MakeExit(room);

				//instantiate new room and remove the clone from its name
				room = Instantiate(room);
				//room.SetActive(false);
				room.transform.parent = mapCreation.mapParent;
				room.name = room.name.Replace("(Clone)", "");

				//move the room to the new position and set it active
				room.transform.SetPositionAndRotation(transform.position, transform.rotation);

				room.SetActive(true);
			}

			spawned = true;

			//Time.timeScale = 0f;
		}
	}

	private GameObject ChangeRoom(GameObject currentRoom)
	{
		var adjRooms = extensions.GetAdjacentRooms(transform.position);

		if (adjRooms["TOP"] && adjRooms["BOTTOM"])
		{
			return templates.closedRoom;
		}
		else if (adjRooms["LEFT"] && adjRooms["RIGHT"])
		{
			return templates.closedRoom;
		}

		return currentRoom;
	}


	private GameObject SpawnClosedRoom()
	{
		//get closed room
		room = templates.closedRoom;
		room = ChangeRoom(room);

		//instantiate new closed room and remove clone
		room = Instantiate(room);
		room.name = room.name.Replace("(Clone)", "");
		room.transform.parent = mapCreation.mapParent;
		//move to position and set active
		room.transform.SetPositionAndRotation(transform.position, transform.rotation);
		room.SetActive(true);
		
		Destroy(gameObject);
		return room;
	}

	private bool CheckIfCanBeExit(GameObject room)
	{
		if ((room.name == "U" || room.name == "D" || room.name == "L" || room.name == "R") && (mapCreation.moveToScenes.Count > 0))
		{
			//print("can be an exit");
			return true;
		}

		return false;
	}

	private GameObject MakeExit(GameObject room)
	{
		//check if its an edge room
		var adjRooms = extensions.GetAdjacentRooms(transform.position);
		//if theres less than 5 empty rooms then return the current room as it would be valid
		if (extensions.CountEmptyRooms(adjRooms) < 4)
		{
			return room;
		}

		//otherwise make the room an exit
		room = EndRoom(room);

		//check if room can be changed into an exit
		if (CheckIfCanBeExit(room))
		{
			//get the according exit dependent on the name of the current room

			//get the new room by adding exit to the end of it
			GameObject newRoom = templates.GetRoom(room.name + "Exit");
			//return exit room
			newRoom.transform.SetPositionAndRotation(room.transform.position, room.transform.rotation);
			return newRoom;
		}

		return room;
	}

	private GameObject EndRoom(GameObject room)
	{
		if (mapCreation.mapParent.childCount >= 50)
		{
			//print("room length is almost at the max: " + GameManager.Instance.thisArea.rooms.Count + " / " + GameManager.Instance.thisArea.maxMapSize);
			//change the room to an end room based on the opening direction
			switch (openingDirection)
			{
				case 1:
					room = templates.GetRoom("D");
					break;
				case 2:
					room = templates.GetRoom("U");
					break;
				case 3:
					room = templates.GetRoom("L");
					break;
				case 4:
					room = templates.GetRoom("R");
					break;
				default:
					print("somehow this code has run - RoomSpawner" + openingDirection);
					break;
			}
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
					Invoke(nameof(SpawnClosedRoom), openingDirection / 50f);
					//Invoke(nameof(SpawnClosedRoom), 0.01f);
					//disable the walls based on the current and collided opening direction
				}

				spawned = true;
			}
			catch{}
		}
	}
}
