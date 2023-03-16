using Assets.Scripts.MapGeneration;
using Assets.Scripts.Pools;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RoomSpawner : MonoBehaviour {

	public int openingDirection;
	// 1 --> need bottom door
	// 2 --> need top door
	// 3 --> need left door
	// 4 --> need right door

	private RoomPool roomPool;
	private RoomTemplates templates;

	private int rand;
	public bool spawned = false;

	public float waitTime = 4f;

	private GameObject room = null;

	void Start(){
		//must destroy instead of setting inactive as the rooms will continue to spawn on top of each other even when set inactive
		//StartCoroutine(Delay());
		Destroy(gameObject, waitTime);

		//roomPool = RoomPool.Instance;
		templates = GameManager.Instance.templates;

		//dont spawn a new room if the max length has been reached
		//if (GameManager.Instance.thisArea.roomsData.Count >= templates.maxRoomLength)
		//{
		//	return;
		//}
		//templates = GameObject.FindGameObjectWithTag("Rooms").GetComponent<RoomTemplates>();

		//switch (transform.GetComponentInParent<AddRoom>().area)
		//{
		//	case Area.RED:
		//		if (templates.roomData.roomsA.Count >= templates.maxRoomLength)
		//		{
		//			return;
		//		}
		//		break;
		//	case Area.GREEN:
		//		if (templates.roomData.roomsB.Count >= templates.maxRoomLength)
		//		{
		//			return;
		//		}
		//		break;
		//	case Area.BLUE:
		//		if (templates.roomData.roomsC.Count >= templates.maxRoomLength)
		//		{
		//			return;
		//		}
		//		break;
		//	case Area.YELLOW:
		//		if (templates.roomData.roomsD.Count >= templates.maxRoomLength)
		//		{
		//			return;
		//		}
		//		break;
		//	case Area.A:
		//		if (templates.roomData.roomsE.Count >= templates.maxRoomLength)
		//		{
		//			return;
		//		}
		//		break;
		//	case Area.B:
		//		if (templates.roomData.roomsF.Count >= templates.maxRoomLength)
		//		{
		//			return;
		//		}
		//		break;
		//	case Area.C:
		//		if (templates.roomData.roomsG.Count >= templates.maxRoomLength)
		//		{
		//			return;
		//		}
		//		break;
		//	case Area.D:
		//		if (templates.roomData.roomsH.Count >= templates.maxRoomLength)
		//		{
		//			return;
		//		}
		//		break;
		//}
		//only spawn a room if the room lengths is not exceeded
		Invoke(nameof(Spawn), 0.1f);
	}

	//should only be called at a random chance
	//private void CompareRooms(GameObject a, GameObject b, int dir)
	//{
	//	//DIR: 1=UP, 2=BOTTOM, 3=RIGHT, 4=LEFT

	//	Transform aPoints = a.transform.Find("SpawnPoints").transform;
	//	Transform bPoints = b.transform.Find("SpawnPoints").transform;


	//	Transform aWalls = a.transform.Find("Walls").transform;
	//	Transform bWalls = b.transform.Find("Walls").transform;

	//	//check each direction for if there're openings in both rooms
	//	if (dir == 1)
	//	{
	//		//UP
	//		//aWalls.Find("North").gameObject.SetActive(false);
	//		//bWalls.Find("South").gameObject.SetActive(false);
	//		if (aPoints.Find("UP").gameObject.activeInHierarchy && bPoints.Find("DOWN").gameObject.activeInHierarchy)
	//		{
	//			//set the walls between the 2 rooms inactive so that the player can move through
	//			aWalls.Find("North").gameObject.SetActive(false);
	//			bWalls.Find("South").gameObject.SetActive(false);

	//			////set the extended variables true so that the camera works according to what the code should in an extended region
	//			////as the other room is included in the extended region both changed rooms should be set to true
	//			//a.GetComponent<AddRoom>().extended = true;
	//			//b.GetComponent<AddRoom>().extended = true;
	//		}
	//		else
	//		{
	//			return;
	//		}
	//	}
	//	else if (dir == 2)
	//	{
	//		//BOTTOM
	//		//aWalls.Find("South").gameObject.SetActive(false);
	//		//bWalls.Find("North").gameObject.SetActive(false);
	//		if (aPoints.Find("DOWN").gameObject.activeInHierarchy && bPoints.Find("UP").gameObject.activeInHierarchy)
	//		{
	//			aWalls.Find("South").gameObject.SetActive(false);
	//			bWalls.Find("North").gameObject.SetActive(false);

	//			//a.GetComponent<AddRoom>().extended = true;
	//			//b.GetComponent<AddRoom>().extended = true;
	//		}
	//		else
	//		{
	//			return;
	//		}
	//	}
	//	else if (dir == 3)
	//	{
	//		//RIGHT
	//		//aWalls.Find("East").gameObject.SetActive(false);
	//		//bWalls.Find("West").gameObject.SetActive(false);
	//		if (aPoints.Find("RIGHT").gameObject.activeInHierarchy && bPoints.Find("LEFT").gameObject.activeInHierarchy)
	//		{
	//			aWalls.Find("East").gameObject.SetActive(false);
	//			bWalls.Find("West").gameObject.SetActive(false);

	//			//a.GetComponent<AddRoom>().extended = true;
	//			//b.GetComponent<AddRoom>().extended = true;
	//		}
	//		else
	//		{
	//			return;
	//		}
	//	}
	//	else if (dir == 4)
	//	{
	//		//LEFT
	//		//aWalls.Find("West").gameObject.SetActive(false);
	//		//bWalls.Find("East").gameObject.SetActive(false);
	//		if (aPoints.Find("LEFT").gameObject.activeInHierarchy && bPoints.Find("RIGHT").gameObject.activeInHierarchy)
	//		{
	//			aWalls.Find("West").gameObject.SetActive(false);
	//			bWalls.Find("East").gameObject.SetActive(false);

	//			//a.GetComponent<AddRoom>().extended = true;
	//			//b.GetComponent<AddRoom>().extended = true;
	//		}
	//		else
	//		{
	//			return;
	//		}
	//	}

	//}

	private IEnumerator Delay()
	{
		yield return new WaitForSeconds(waitTime);
		//destroy room spawner so it doesnt keep spawning rooms
		//set the gameobject inactive
		//Destroy(gameObject.GetComponent<RoomSpawner>());
		//gameObject.SetActive(false);

		//set the parent SpawnPoints inactive
		//transform.parent.gameObject.SetActive(false);
	}

	//spawning the next room
	void Spawn(){
		if(spawned == false){
			//GameObject currentRoom = transform.parent.parent.gameObject;

			if(openingDirection == 1){
				// Need to spawn a room with a BOTTOM door.
				rand = Random.Range(0, templates.bottomRooms.Length);
				room = RoomPool.Instance.GetPooledRoom(templates.bottomRooms[rand].name);
				//room.transform.SetPositionAndRotation(transform.position, transform.rotation);

				//CompareRooms(currentRoom, room, openingDirection);
				//room = Instantiate(templates.bottomRooms[rand], transform.position, templates.bottomRooms[rand].transform.rotation);

			} else if(openingDirection == 2){
				// Need to spawn a room with a TOP door.
				rand = Random.Range(0, templates.topRooms.Length);
				room = RoomPool.Instance.GetPooledRoom(templates.topRooms[rand].name);
				//room.transform.SetPositionAndRotation(transform.position, transform.rotation);

				//CompareRooms(currentRoom, room, openingDirection);
				//room = Instantiate(templates.topRooms[rand], transform.position, templates.topRooms[rand].transform.rotation);

			} else if(openingDirection == 3){
				// Need to spawn a room with a LEFT door.
				rand = Random.Range(0, templates.leftRooms.Length);
				room = RoomPool.Instance.GetPooledRoom(templates.leftRooms[rand].name);
				//room.transform.SetPositionAndRotation(transform.position, transform.rotation);

				//CompareRooms(currentRoom, room, openingDirection);
				//room = Instantiate(templates.leftRooms[rand], transform.position, templates.leftRooms[rand].transform.rotation);

			} else if(openingDirection == 4){
				// Need to spawn a room with a RIGHT door.
				rand = Random.Range(0, templates.rightRooms.Length);
				room = RoomPool.Instance.GetPooledRoom(templates.rightRooms[rand].name);
				//room.transform.SetPositionAndRotation(transform.position, transform.rotation);

				//CompareRooms(currentRoom, room, openingDirection);
				//room = Instantiate(templates.rightRooms[rand], transform.position, templates.rightRooms[rand].transform.rotation);

			}

			//rand = Random.Range(0, 2);
			//if (rand == 0)
			//{
			//	CompareRooms(currentRoom, room, openingDirection);
			//}
			//check that the room isnt null


			room.transform.SetPositionAndRotation(transform.position, transform.rotation);
			room.SetActive(true);
			GameManager.Instance.thisArea.roomsData.Add(new RoomData(room.transform.position, room.transform.rotation, room.name, null, null));



			//rand = Random.Range(0, 10);

			//if (room != null)
			//{
			//	//set the room details of the next room - should always occur when a room is spawned
			//	//SetRoomDetails(room);

			//	//random chance to compare and extend the rooms
			//	if (rand <= 3)
			//	{
			//		//CompareRooms(currentRoom, room, openingDirection);
			//	}
			//}
			spawned = true;

			//Destroy(gameObject.GetComponent<RoomSpawner>());
			gameObject.SetActive(false);
			//transform.parent.gameObject.SetActive(false);
		}
	}

	//private Color GetSelfColour()
	//{
	//	var colour = transform.parent.parent.GetChild(0).GetChild(0).GetComponent<Tilemap>().color;
	//	return colour;
	//}

	//private Area GetSelfArea()
	//{
	//	var area = transform.parent.parent.GetComponent<AddRoom>().area;
	//	return area;
	//}

	//private void SetRoomDetails(GameObject room)
	//{
	//	Color color = GetSelfColour();

	//	Tilemap[] tilemaps = room.transform.GetChild(0).GetComponentsInChildren<Tilemap>();

	//	foreach (Tilemap tilemap in tilemaps)
	//	{
	//		tilemap.color = color;
	//	}

	//	room.GetComponent<AddRoom>().area = GetSelfArea();
	//}

	void OnTriggerEnter2D(Collider2D other){
		//occurs when 2 rooms attempt to spawn a room at the same area
		if(other.CompareTag("SpawnPoint") && other.name != "Room"){
			try
			{
				//print(other.name);
				//if ((other.transform.GetComponentInParent<AddRoom>().area != transform.GetComponentInParent<AddRoom>().area) && other.name == "CENTRE")
				//{
				//	Debug.Log(other.transform.GetComponentInParent<AddRoom>().area.ToString() + transform.GetComponentInParent<AddRoom>().area.ToString());
				//	//if theyre different areas
				//	//get the dir to determine which direction to delete
				//	CompareRoomsB(transform.parent.parent.gameObject, other.transform.parent.parent.gameObject, openingDirection);
				//	spawned = true;
				//	return;
				//}



				if (other.GetComponent<RoomSpawner>() != null && other.GetComponent<RoomSpawner>().spawned == false && spawned == false)
				{
					room = roomPool.GetPooledRoom(templates.closedRoom.name);
					//room = roomPool.GetPooledObject(templates.connectRooms[Random.Range(0, templates.connectRooms.Length)]);
					room.transform.SetPositionAndRotation(transform.position, transform.rotation);
					room.SetActive(true);
					GameManager.Instance.thisArea.roomsData.Add(new RoomData(transform.position, Quaternion.identity, room.name, null, null));

					//GameObject room = Instantiate(templates.closedRoom);
					//SetRoomDetails(room);

					//Destroy(gameObject.GetComponent<RoomSpawner>());
					//gameObject.SetActive(false);
					//transform.parent.gameObject.SetActive(false);
				}
				else
				{
					//CompareRooms(gameObject, other.gameObject, openingDirection);
				}
				spawned = true;
				Destroy(gameObject);


				//if (other.GetComponent<RoomSpawner>().spawned == false && spawned == false)
				//{
				//	room = objectPool.GetPooledObject(templates.closedRoom);
				//	room.transform.SetPositionAndRotation(transform.position, transform.rotation);
				//	//GameObject room = Instantiate(templates.closedRoom);
				//	SetRoomDetails(room);

				//	Destroy(gameObject.GetComponent<RoomSpawner>());
				//	gameObject.SetActive(false);
				//	//Destroy(gameObject);
				//}
				//spawned = true;
			}
			catch
			{
				print("exception");
			}
		}
	}
}
