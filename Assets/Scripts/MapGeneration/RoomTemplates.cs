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


	private List<GameObject> rooms = GameManager.Instance.thisArea.rooms;
	private List<RoomData> roomsData = GameManager.Instance.thisArea.roomsData;

	private void Start()
	{
		//roomData = GameManager.Instance.roomData;

		//InvokeRepeating("CheckRoomLengths", 0f, 0.05f);


		//Invoke(nameof(SpawnBosses), 10f);
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
