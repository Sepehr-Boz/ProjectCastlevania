using Assets.Scripts.MapGeneration;
using Assets.Scripts.Pools;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
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


		//Invoke(nameof(Spawn), 0.1f);
		Invoke(nameof(Spawn), openingDirection / 10f);
	}

	//private IEnumerator Delay()
	//{
	//	yield return new WaitForSeconds(waitTime);
	//	gameObject.SetActive(false);
	//}

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

				room.transform.SetPositionAndRotation(transform.position, transform.rotation);
				room = ChangeRoom(room);

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
				//else if ("UDLR".Contains(room.name) && templates.GetAdjacentRooms(room).Length >= 5) //cant do this because every new spawned room will have more than 5 empty neightbours
				//{
				//	room = RoomPool.Instance.GetPooledRoom(templates.exitRoom.name);
				//}

				room.transform.SetPositionAndRotation(transform.position, transform.rotation);
				room.SetActive(true);
				List<Wall> walls = new List<Wall> { Wall.NORTH, Wall.EAST, Wall.SOUTH, Wall.WEST };
				GameManager.Instance.thisArea.roomsData.Add(new RoomData(room.transform.position, room.transform.rotation, walls, room.name, new List<GameObject>(), new List<GameObject>()));

				//spawned = true;
			}

			spawned = true;
			//spawned = true;
			//Destroy(gameObject);
			//gameObject.SetActive(false);
		}
	}

	private GameObject ChangeRoom(GameObject currentRoom)
	{
		//get adjascent rooms around the spawn point - top, left, right, bottom specifically
		Dictionary<string, GameObject> adjRooms = templates.GetAdjacentRooms(currentRoom);
		//GameObject[] adjRooms = templates.GetAdjacentRooms(currentRoom);
		//adjRooms = new GameObject[4] { adjRooms[1], adjRooms[3], adjRooms[5], adjRooms[7] };

		//check if current room is valid
		bool valid = true;
		string newRoomName = "";

		foreach (char dir in currentRoom.name)
		{
			if (dir == "L"[0] && (adjRooms["LEFT"] == null || adjRooms["LEFT"].name.Contains("R")))
			{
				newRoomName += "L";
			}
			else if (dir == "R"[0] && (adjRooms["RIGHT"] == null || adjRooms["RIGHT"].name.Contains("L")))
			{
				newRoomName += "R";
			}
			else if (dir == "U"[0] && (adjRooms["TOP"] == null || adjRooms["TOP"].name.Contains("D")))
			{
				newRoomName += "U";
			}
			else if (dir == "D"[0] && (adjRooms["BOTTOM"] == null || adjRooms["BOTTOM"].name.Contains("U")))
			{
				newRoomName += "D";
			}
			else
			{
				valid = false;
			}
		}
		//	else if (dir == "U"[0])
		//	{
		//		if (adjRooms["TOP"] == null || adjRooms["TOP"].name.Contains("D"))
		//		{
		//			newRoomName += "U";
		//			continue;
		//		}
		//		else
		//		{
		//			valid = false;
		//		}
		//	}
		//	else if (dir == "D"[0])
		//	{
		//		if (adjRooms["BOTTOM"] == null || adjRooms["BOTTOM"].name.Contains("U"))
		//		{
		//			newRoomName += "D";
		//			continue;
		//		}
		//		else
		//		{
		//			valid = false;
		//		}
		//	}
		//}

		if (valid || newRoomName.Equals(currentRoom.name))
		{
			print("room was valid: " + currentRoom.name + " new room name was : " + newRoomName + " whether it was valid : " + valid.ToString());
			//print("connected rooms are: " + adjRooms["TOP"] + " " + adjRooms["LEFT"].name + " " + adjRooms["RIGHT"].name + " " + adjRooms["BOTTOM"].name);
			return currentRoom;
		}

		print("current room wasnt valid : " + currentRoom.name + "new room to spawn is " + newRoomName);
		Time.timeScale = 0.001f;

		GameObject newRoom = RoomPool.Instance.GetPooledRoom(newRoomName);
		newRoom.transform.SetPositionAndRotation(currentRoom.transform.position, currentRoom.transform.rotation);
		return newRoom;


		//order in which characters are added should be U D L R always otherwise it wont get the room correctly
		//get the new room name needed
		//string newRoomName = "";
		//if (adjRooms["TOP"] != null && adjRooms["TOP"].name.Contains("D"))
		//{
		//	newRoomName += "U";
		//}
		//if (adjRooms["BOTTOM"] != null && adjRooms["BOTTOM"].name.Contains("U"))
		//{
		//	newRoomName += "D";
		//}
		//if (adjRooms["LEFT"] != null && adjRooms["LEFT"].name.Contains("R"))
		//{
		//	newRoomName += "L";
		//}
		//if (adjRooms["RIGHT"] != null && adjRooms["RIGHT"].name.Contains("L"))
		//{
		//	newRoomName += "R";
		//}
		//string newRoomName = "";
		//if (adjRooms["TOP"] == null || adjRooms["TOP"].name.Contains("D"))
		//{
		//	newRoomName += "U";
		//}
		//if (adjRooms["BOTTOM"] == null || adjRooms["BOTTOM"].name.Contains("U"))
		//{
		//	newRoomName += "D";
		//}
		//if (adjRooms["LEFT"] == null || adjRooms["LEFT"].name.Contains("R"))
		//{
		//	newRoomName += "L";
		//}
		//if (adjRooms["RIGHT"] == null || adjRooms["RIGHT"].name.Contains("L"))
		//{
		//	newRoomName += "R";
		//}
		////string newRoomName = "";
		////if (adjRooms[0] == null || adjRooms[0].name.Contains("D"))
		////{
		////	newRoomName += "U";
		////}
		////if (adjRooms[3] == null || adjRooms[1].name.Contains("U"))
		////{
		////	newRoomName += "D";
		////}
		////if (adjRooms[1] == null || adjRooms[2].name.Contains("R"))
		////{
		////	newRoomName += "L";
		////}
		////if (adjRooms[2] == null || adjRooms[3].name.Contains("L"))
		////{
		////	newRoomName += "R";
		////}

		////check if the new room name is the same as the current room name, or if there are no neighbours yet
		//if (newRoomName.Equals(currentRoom.name) || newRoomName.Equals(""))
		//{
		//	//if it is then return the current room
		//	print("room is fine");
		//	return currentRoom;
		//}

		////1: top 2: bottom 3:right 4:left //0: top 1: left 2: right 3: bottom
		////else if (openingDirection == 1 && (adjRooms[0] == null && adjRooms[1] == null && adjRooms[2] == null))
		////{
		////	print("top room is fine");
		////	return currentRoom;
		////}
		////else if (openingDirection == 2 && (adjRooms[3] == null && adjRooms[1] == null && adjRooms[2] == null))
		////{
		////	print("bottom room is fine");
		////	return currentRoom;
		////}
		////else if (openingDirection == 3 && (adjRooms[2] == null && adjRooms[0] == null && adjRooms[3] == null))
		////{
		////	print("right room is fine");
		////	return currentRoom;
		////}
		////else if (openingDirection == 4 && (adjRooms[1] == null && adjRooms[0] == null && adjRooms[3] == null))
		////{
		////	print("left room is fine");
		////	return currentRoom;
		////}
		////print("new room name is" + newRoomName + "names = " + String.Join(" ", new List<string>(adjRooms).ToArray()));
		//print("new room spawned : " + newRoomName);
		////if not then get a room with the new name from RoomPool
		//GameObject newRoom = RoomPool.Instance.GetPooledRoom(newRoomName);
		////set the position and rotation of the new room
		//newRoom.transform.SetPositionAndRotation(currentRoom.transform.position, currentRoom.transform.rotation);
		//return newRoom;
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
					//print(transform.root.gameObject.name + " has collided with " + other.name);
					Invoke(nameof(SpawnClosedRoom), openingDirection / 10f);
					//Invoke(nameof(SpawnClosedRoom), 0.1f);
					//room = RoomPool.Instance.GetPooledRoom(templates.closedRoom.name);
					//room.transform.SetPositionAndRotation(transform.position, transform.rotation);
					//room.SetActive(true);
					//List<Wall> walls = new List<Wall> { Wall.NORTH, Wall.EAST, Wall.SOUTH, Wall.WEST };
					//GameManager.Instance.thisArea.roomsData.Add(new RoomData(transform.position, Quaternion.identity, walls, room.name, new List<GameObject>(), new List<GameObject>()));
					//Destroy(gameObject);
					//spawned = true;
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
