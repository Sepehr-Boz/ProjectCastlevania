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
	private RoomTemplates templates;
	private MapCreation mapCreation;
	private ExtensionMethods extensions;

	private GameObject room = null;

	[Header("Spawning")]
	private int rand;
	public bool spawned = false;

	public float waitTime = 4f;

	void Start()
	{
		templates = GameManager.Instance.templates;
		mapCreation = GameManager.Instance.mapCreation;
		extensions = GameManager.Instance.extensions;

		newEntryChance = extensions.newEntryChance;

		//must destroy instead of setting inactive as the rooms will continue to spawn on top of each other even when set inactive
		Destroy(gameObject, waitTime);

		//stagger the spawn times otherwise the game lags quite a bit
		Invoke(nameof(Spawn), openingDirection / 50f);
	}

	//spawning the next room
	void Spawn(){
		if(spawned == false){
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
				int rand = Random.Range(0, 100);
				if (rand <= newEntryChance)
				{
					room = templates.openRoom;
				}
				room = ChangeRoom(room);
				//instantiate new room and remove the clone from its name
				room = Instantiate(room);
				room.transform.parent = mapCreation.mapParent;
				//move the room to the new position and set it active
				room.transform.SetPositionAndRotation(transform.position, transform.rotation);

				switch (openingDirection) //need this otherwise rooms are spawned behind into previous room
				{
					case 1:
						Destroy(room.transform.Find("SpawnPoints").Find("DOWN").gameObject);
						break;
					case 2:
						Destroy(room.transform.Find("SpawnPoints").Find("UP").gameObject);
						break;
					case 3:
						Destroy(room.transform.Find("SpawnPoints").Find("LEFT").gameObject);
						break;
					case 4:
						Destroy(room.transform.Find("SpawnPoints").Find("RIGHT").gameObject);
						break;
				}

				room.SetActive(true);
			}
			spawned = true;
		}
	}

	private GameObject ChangeRoom(GameObject currentRoom)
	{
		//need to get unfiltered room as this function checks for corridors and GetAdjacentRooms() regular doesn't pass in corridors
		var adjRooms = extensions.GetAdjacentRoomsUnfiltered(transform.position);

		if (adjRooms["TOP"] && adjRooms["BOTTOM"])
		{
			return templates.closedRoom;
		}
		else if (adjRooms["LEFT"] && adjRooms["RIGHT"])
		{
			return templates.closedRoom;
		}
		else if (adjRooms["TOP"] && adjRooms["TOP"].name.Equals("LR--")) //check for specific corridors as if theres any corridors then there shouldnt be any openings to it
		{
			return templates.closedRoom;
		}
		else if (adjRooms["BOTTOM"] && adjRooms["BOTTOM"].name.Equals("LR--"))
		{
			return templates.closedRoom;
		}
		else if (adjRooms["LEFT"] && adjRooms["LEFT"].name.Equals("UD--"))
		{
			return templates.closedRoom;
		}
		else if (adjRooms["RIGHT"] && adjRooms["RIGHT"].name.Equals("UD--"))
		{
			return templates.closedRoom;
		}

		return currentRoom;
	}


	private GameObject SpawnClosedRoom()
	{
		//get closed room
		room = templates.closedRoom;
		//instantiate new closed room and remove clone
		room = Instantiate(room);
		room.transform.parent = mapCreation.mapParent;
		//move to position and set active
		room.transform.SetPositionAndRotation(transform.position, transform.rotation);
		room.SetActive(true);

		Destroy(gameObject);
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
				}
				spawned = true;
			}
			catch{}
		}
	}
}
