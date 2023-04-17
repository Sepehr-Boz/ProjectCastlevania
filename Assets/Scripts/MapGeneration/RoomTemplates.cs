using Assets.Scripts.MapGeneration;
using Assets.Scripts.Pools;
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
	public GameObject openRoom;
	public GameObject bossRoom;
	//public GameObject[] exitRooms;

	public GameObject[] allRooms; //used when spawning rooms from rooms data

	//make room templates a singleton as it is the most commonly and referenced object in every scene
	//#region singleton
	//private static RoomTemplates _instance;

	//public static RoomTemplates Instance
	//{
	//	get
	//	{
	//		return _instance;
	//	}
	//}

	//private void Awake()
	//{
	//	_instance = this;
	//}
	//#endregion

	#region getting rooms

	//substitute for room pool so room pool can be removed from the scenes and room templates can be used instead as rooms will be instantiated instead of pooled
	public GameObject GetRoom(string roomName = null)
	{
		//loop through all rooms and return the prefab that is needed
		foreach (GameObject room in allRooms)
		{
			if (room.name.Equals(roomName))
			{
				//print("room found - GetRoom");
				return room;
			}
		}
		print("room tried to get - SO MAKE ONE - is : " + roomName);
		//if nothing is passed then return null
		//if no room is valid then return a closed room instead
		return closedRoom;
	}

	//public GameObject GetExitRoom(string exitName = null)
	//{
	//	foreach (GameObject room in exitRooms)
	//	{
	//		if (room.name.Equals(exitName))
	//		{
	//			return room;
	//		}
	//	}

	//	return null;
	//}

	#endregion
}
