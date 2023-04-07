﻿using Assets.Scripts.MapGeneration;
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
		newEntryChance = templates.newEntryChance;

		Invoke(nameof(Spawn), openingDirection / 100f);
	}

	//spawning the next room
	void Spawn(){
		if(spawned == false){
			//GameObject currentRoom = transform.parent.parent.gameObject;

			if(openingDirection == 1){
				// Need to spawn a room with a BOTTOM door.
				rand = Random.Range(0, templates.bottomRooms.Length);
				room = RoomTemplates.Instance.bottomRooms[rand];
			} 
			else if(openingDirection == 2){
				// Need to spawn a room with a TOP door.
				rand = Random.Range(0, templates.topRooms.Length);
				room = RoomTemplates.Instance.topRooms[rand];
			}
			else if(openingDirection == 3){
				// Need to spawn a room with a LEFT door.
				rand = Random.Range(0, templates.leftRooms.Length);
				room = RoomTemplates.Instance.leftRooms[rand];
			}
			else if(openingDirection == 4){
				// Need to spawn a room with a RIGHT door.
				rand = Random.Range(0, templates.rightRooms.Length);
				room = RoomTemplates.Instance.rightRooms[rand];
			}

			//check that the room isnt null
			if (room != null)
			{
				//check if room can be an exit
				room = MakeExit(room);

				//have chance to replace the room with an open room which will enable the map to extend further as the current open room (UDRL) has 4 exits
				int rand = Random.Range(0, 100);
				//check if room should be an entry room
				if (rand <= newEntryChance || templates.startRooms.Contains((Vector2)transform.position))
				{
					room = RoomTemplates.Instance.openRoom;
				}
				//check if room should be a boss room
				else if (templates.bossRooms.Contains((Vector2)transform.position))
				{
					room = RoomTemplates.Instance.bossRoom;
				}

				//instantiate new room and remove the clone from its name
				room = Instantiate(room);
				room.name = room.name.Replace("(Clone)", "");

				//move the room to the new position and set it active
				room.transform.SetPositionAndRotation(transform.position, transform.rotation);
				room.SetActive(true);
				//add new room to room data
				List<Wall> walls = new List<Wall> { Wall.NORTH, Wall.EAST, Wall.SOUTH, Wall.WEST };
				GameManager.Instance.thisArea.roomsData.Add(new RoomData(room.transform.position, room.transform.rotation, walls, room.name, new List<GameObject>(), new List<GameObject>()));
			}

			spawned = true;
		}
	}


	private void SpawnClosedRoom()
	{
		//get closed room
		room = RoomTemplates.Instance.closedRoom;
		//instantiate new closed room and remove clone
		room = Instantiate(room);
		room.name = room.name.Replace("(Clone)", "");
		//move to position and set active
		room.transform.SetPositionAndRotation(transform.position, transform.rotation);
		room.SetActive(true);
		//add to room data
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

		//check if room can be changed into an exit
		if (CheckIfCanBeExit(room))
		{
			//get the according exit dependent on the name of the current room
			GameObject newRoom = null;
			if (room.name.Equals("U"))
			{
				newRoom = RoomTemplates.Instance.GetExitRoom("UExit");
			}
			else if (room.name.Equals("D"))
			{
				newRoom = RoomTemplates.Instance.GetExitRoom("DExit");
			}
			else if (room.name.Equals("L"))
			{
				newRoom = RoomTemplates.Instance.GetExitRoom("LExit");
			}
			else if (room.name.Equals("R"))
			{
				newRoom = RoomTemplates.Instance.GetExitRoom("RExit");
			}
			//return exit room
			newRoom.transform.SetPositionAndRotation(room.transform.position, room.transform.rotation);
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
					Invoke(nameof(SpawnClosedRoom), openingDirection / 100f);
				}

				spawned = true;
			}
			catch{}
		}
	}
}
