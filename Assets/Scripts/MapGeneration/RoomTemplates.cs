using Assets.Scripts.Pools;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class RoomTemplates : MonoBehaviour 
{
	[Header("Room References")]
	[Space(2)]
	public GameObject[] bottomRooms;
	public GameObject[] topRooms;
	public GameObject[] leftRooms;
	public GameObject[] rightRooms;
	public GameObject closedRoom;

	public GameObject boss;

	//[SerializeField]public RoomData roomData;

	[Header("Script Variables")]
	//public float waitTime;
	public int maxRoomLength = 50; //accessed from roomspawner also
	public int numBosses = 1; //how many bosses to spawn


	private List<GameObject> rooms;
	private List<RoomData> roomsData;

	private void Start()
	{
		//roomData = GameManager.Instance.roomData;

		//InvokeRepeating("CheckRoomLengths", 0f, 0.05f);

		//check if rooms are empty
		if (AreRoomsEmpty())
		{
			print("rooms are empty");

			rooms = GameManager.Instance.thisArea.rooms;

			for (int i = 0; i < 4; i++)
			{
				//set the start rooms active if the rooms data are empty
				rooms[i].transform.Find("SpawnPoints").gameObject.SetActive(true);
				foreach (Transform child in rooms[i].transform.Find("SpawnPoints"))
				{
					child.gameObject.SetActive(true);
				}
				//GameManager.Instance.thisArea.rooms[i].SetActive(true);
			}
		}
		else
		{
			print("rooms arent empty");

			//if the rooms arent empty then set all spawnpoints inactive so rooms dont keep spawning
			//List<GameObject> rooms = GameManager.Instance.thisArea.rooms;
			//int n = GameManager.Instance.thisArea.roomsData.Count;
			List<RoomData> roomsData = GameManager.Instance.thisArea.roomsData;
			//print(n);
			GameObject tmp;

			foreach (RoomData data in roomsData)
			{
				print(data);
				tmp = RoomPool.Instance.GetPooledRoom(data.name);
				//set the SpawnPoints parent false so that the points stop spawning rooms
				tmp.transform.Find("SpawnPoints").gameObject.SetActive(false);
				tmp.transform.SetPositionAndRotation(data.position, data.rotation);

				GameManager.Instance.thisArea.rooms.Add(tmp);

				tmp.SetActive(true);
			}

			//for (int i = 0; i < n; i++)
			//{
			//	//get a pooled object with the same name
			//	tmp = RoomPool.Instance.GetPooledObject(null, roomsData[i].name);
			//	//tmp.transform.Find("SpawnPoints").gameObject.SetActive(false);
			//	//Destroy(tmp.transform.Find("SpawnPoints").gameObject);
			//	//tmp.transform.Find("SpawnPoints").gameObject.SetActive(false);
			//	print("a");
			//	tmp.transform.SetPositionAndRotation(roomsData[i].position, roomsData[i].rotation);
			//	print("b");
			//	//tmp.transform.position = roomsData[i].position;
			//	//tmp.transform.rotation = roomsData[i].rotation;

			//	tmp.SetActive(true);
			//	print("e");
			//	GameManager.Instance.thisArea.rooms.Add(tmp);
			//	print("f");


			//	//set the spawn points of the room inactive
			//	//rooms[i].transform.Find("SpawnPoints").gameObject.SetActive(false);
			//	//foreach (Transform child in rooms[i].transform.Find("SpawnPoints"))
			//	//{
			//	//	Destroy(child.gameObject);
			//	//}
			//	////move the room to the position in roomsdata
			//	//GameManager.Instance.thisArea.rooms[i].transform.position = GameManager.Instance.thisArea.roomsData[i].position;
			//	//GameManager.Instance.thisArea.rooms[i].transform.rotation = GameManager.Instance.thisArea.roomsData[i].rotation;
			//	////rooms[i].transform.position = roomsData[i].position;
			//	////rooms[i].transform.rotation = roomsData[i].rotation;

			//	////spawn enemies and objects into room IF ROOM ISNT AN ENTRY ONE
			//}

			////foreach (GameObject room in GameManager.Instance.thisArea.rooms)
			////{
			////	room.transform.Find("SpawnPoints").gameObject.SetActive(false);
			////}
		}


		Invoke(nameof(SpawnBosses), 10f);
	}

	private void OnApplicationQuit()
	{
		GameManager.Instance.thisArea.rooms.Clear();

	}

	//check if room data in the area isnt empty
	private bool AreRoomsEmpty()
	{
		return GameManager.Instance.thisArea.roomsData.Count < 5;
		//returns true if empty, false if not empty
	}

	#region depreceated
	//depreceated as the room lengths are checked in room spawner so the list lengths never exceed the maxroomlength
	//private void CheckRoomLengths()
	//{
	//	if (roomData.roomsA.Count > maxRoomLength + 1)
	//	{
	//		roomData.roomsA[maxRoomLength].SetActive(false);
	//		//Destroy(roomData.roomsA[maxRoomLength]);
	//		roomData.roomsA.RemoveAt(maxRoomLength);
	//		return;
	//	}
	//	if (roomData.roomsB.Count > maxRoomLength + 1)
	//	{
	//		roomData.roomsB[maxRoomLength].SetActive(false);
	//		//Destroy(roomData.roomsB[maxRoomLength]);
	//		roomData.roomsB.RemoveAt(maxRoomLength);
	//		return;
	//	}
	//	if (roomData.roomsC.Count > maxRoomLength + 1)
	//	{
	//		roomData.roomsC[maxRoomLength].SetActive(false);
	//		//Destroy(roomData.roomsC[maxRoomLength]);
	//		roomData.roomsC.RemoveAt(maxRoomLength);
	//		return;
	//	}
	//	if (roomData.roomsD.Count > maxRoomLength + 1)
	//	{
	//		roomData.roomsD[maxRoomLength].SetActive(false);
	//		//Destroy(roomData.roomsD[maxRoomLength]);
	//		roomData.roomsD.RemoveAt(maxRoomLength);
	//		return;
	//	}
	//	if (roomData.roomsA.Count < maxRoomLength || roomData.roomsB.Count < maxRoomLength || roomData.roomsC.Count < maxRoomLength || roomData.roomsD.Count < maxRoomLength)
	//	{
	//		return;
	//	}

	//	CancelInvoke("CheckRoomLengths");

	//}
	#endregion


	//should be called when the rooms have been generated
	public void SpawnBosses()
	{

		////rand should be between half roomsA length not max room length as the length of the rooms does not always reach max room lengths so may be shorter
		//rand = Random.Range(Mathf.RoundToInt(roomData.roomsA.Count / 2), roomData.roomsA.Count);
		//roomBoss = Instantiate(boss, roomData.roomsA[rand].transform.position, Quaternion.identity);
		//roomData.roomsA.Add(roomBoss);

		//rand = Random.Range(Mathf.RoundToInt(roomData.roomsB.Count / 2), roomData.roomsB.Count);
		//roomBoss = Instantiate(boss, roomData.roomsB[rand].transform.position, Quaternion.identity);
		//roomData.roomsB.Add(roomBoss);

		//rand = Random.Range(Mathf.RoundToInt(roomData.roomsC.Count / 2), roomData.roomsC.Count);
		//roomBoss = Instantiate(boss, roomData.roomsC[rand].transform.position, Quaternion.identity);
		//roomData.roomsC.Add(roomBoss);

		//rand = Random.Range(Mathf.RoundToInt(roomData.roomsD.Count / 2), roomData.roomsD.Count);
		//roomBoss = Instantiate(boss, roomData.roomsD[rand].transform.position, Quaternion.identity);
		//roomData.roomsD.Add(roomBoss);


		for (int i = 0; i < numBosses; i++)
		{
			int rand = Random.Range(Mathf.RoundToInt(rooms.Count / 2), rooms.Count);
			GameObject areaBoss = Instantiate(boss, rooms[rand].transform.position, Quaternion.identity);
			roomsData[rand].AddEnemy(areaBoss);
		}

	}
}
