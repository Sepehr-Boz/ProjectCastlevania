using Assets.Scripts.MapGeneration;
using Assets.Scripts.Pools;
using Unity.VisualScripting;
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
	public GameObject[] allRooms;
	public GameObject focus;


	//substitute for room pool so room pool can be removed from the scenes and room templates can be used instead as rooms will be instantiated instead of pooled
	public GameObject GetRoom(string roomName = null)
	{
		roomName = OrderNameCorrectly(roomName);

		//loop through all rooms and return the prefab that is needed
		foreach (GameObject room in allRooms)
		{
			if (room.name.Equals(roomName))
			{
				return room;
			}
		}
		print("room tried to get - SO MAKE ONE - is : " + roomName);
		//if nothing is passed then return null
		//if no room is valid then return a closed room instead
		return closedRoom;
	}

	public string OrderNameCorrectly(string name)
	{
		//check if there are multiple occurrences of U, D, L, R
		int n = name.Length;
		string nameTemp = "";
		for (int i = 0; i < n; i++)
		{
			//only add to nameTemp if a character isnt already in it so it makes sure there arent repeats
			nameTemp += nameTemp.Contains(name[i]) ? name[i] : "";
		}

		//check the order it should be in order: U -> D -> L -> R
		name = "UDLR";
		//remove nameTemp from full
		foreach (char c in name)
		{
			if (nameTemp.Contains(c))
			{
				continue;
			}

			name = name.Replace(c, ' ');
		}

		return name;
	}
}
