using Assets.Scripts.MapGeneration;
using Assets.Scripts.Pools;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

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
		//StartCoroutine(Delay());

		templates = GameManager.Instance.templates;
		newEntryChance = templates.newEntryChance;

		//only spawn a room if the room lengths is not exceeded
		//Invoke(nameof(Spawn), 0.1f);

		//if (GameManager.Instance.thisArea.roomsData.Count < templates.maxRoomLength)
		//{
		//	//Spawn();
		//	//spawn the room after a different time for each direction so that the processor maaaybe has a lower burden as it needs to do less operations every second?
		//	Invoke(nameof(Spawn), openingDirection / 100f);
		//}
		//else
		//{
		//	//gameObject.SetActive(false);
		//	//Destroy(gameObject);
		//}
		Invoke(nameof(Spawn), openingDirection / 100f);
	}

	private IEnumerator Delay()
	{
		yield return new WaitForSeconds(waitTime);
		gameObject.SetActive(false);
	}

	//spawning the next room
	void Spawn(){
		//if (GameManager.Instance.thisArea.roomsData.Count >= templates.maxRoomLength)
		//{

		//	return;
		//}



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
				//have chance to replace the room with an open room which will enable the map to extend further as the current open room (UDRL) has 4 exits
				int rand = Random.Range(0, 500);
				if (rand <= newEntryChance)
				{
					print("open room has replaced da room");
					room = RoomPool.Instance.GetPooledRoom(templates.openRoom.name);
				}

				room.transform.SetPositionAndRotation(transform.position, transform.rotation);
				room.SetActive(true);
				List<Wall> walls = new List<Wall> { Wall.NORTH, Wall.EAST, Wall.SOUTH, Wall.WEST };
				GameManager.Instance.thisArea.roomsData.Add(new RoomData(room.transform.position, room.transform.rotation, walls, room.name, new List<GameObject>(), new List<GameObject>()));
			}

			spawned = true;
			//spawned = true;
			//Destroy(gameObject);
			//gameObject.SetActive(false);
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

	void OnTriggerEnter2D(Collider2D other){
		//occurs when 2 rooms attempt to spawn a room at the same area
		if(other.CompareTag("SpawnPoint")){
			try
			{
				if (other.GetComponent<RoomSpawner>().spawned == false && spawned == false)
				{
					print(transform.root.gameObject.name + " has collided with " + other.name);
					Invoke(nameof(SpawnClosedRoom), openingDirection / 100f);
					//room = RoomPool.Instance.GetPooledRoom(templates.closedRoom.name);
					//room.transform.SetPositionAndRotation(transform.position, transform.rotation);
					//room.SetActive(true);
					//List<Wall> walls = new List<Wall> { Wall.NORTH, Wall.EAST, Wall.SOUTH, Wall.WEST };
					//GameManager.Instance.thisArea.roomsData.Add(new RoomData(transform.position, Quaternion.identity, walls, room.name, new List<GameObject>(), new List<GameObject>()));
					//Destroy(gameObject);
				}
				spawned = true;
				//gameObject.SetActive(false);
			}
			catch
			{
				print("other rooms spawener: " + other.GetComponent<RoomSpawner>().spawned);
				print("self room spawner is : " + spawned);
				print("the other rooms name is " + other.name);
				print("exception");
			}
		}
		else
		{
			//if the space ahead where the room spawner plans to spawn is occupied
			//if so then change the room?
		}
	}
}
