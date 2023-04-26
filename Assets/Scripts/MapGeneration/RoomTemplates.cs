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

	public GameObject[] bossRooms;
	public GameObject[] allRooms; //used when spawning rooms from rooms data
	public GameObject focus;
	public GameObject exits;


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
}
