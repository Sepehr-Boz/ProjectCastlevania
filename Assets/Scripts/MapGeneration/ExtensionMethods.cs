using Assets.Scripts.MapGeneration;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Events;
using UnityEngine;
using System.Linq.Expressions;
using Unity.VisualScripting;

public class ExtensionMethods : MonoBehaviour
{
	private readonly Vector2[] directions =
	{
		new Vector2(-10, 10),new Vector2(0, 10),new Vector2(10, 10),
		new Vector2(-10, 0),new Vector2(0, 0),new Vector2(10, 0),
		new Vector2(-10, -10),new Vector2(0, -10),new Vector2(10, -10),
	};


	public UnityEvent<Dictionary<string, GameObject>> extendFunction = new();

	[Range(0, 100)]
	public int newEntryChance = 1;


	//#region singleton
	//private static ExtensionMethods _instance;

	//public static ExtensionMethods Instance
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


	#region extending rooms

	public Dictionary<string, GameObject> GetAdjacentRooms(Vector2 currentPos)
	{
		//check if currentroom has already been extended and if it has then return
		//check for any rooms above, below, to the right, left, and diagonally of the current room
		Dictionary<string, GameObject> rooms = new(9)
		{
			{"TOPLEFT", null },  {"TOP", null }, {"TOPRIGHT", null },
			{"LEFT", null }, {"CENTRE", null }, {"RIGHT", null },
			{"BOTTOMLEFT", null }, {"BOTTOM", null }, {"BOTTOMRIGHT", null }
		};

		Vector2 newPos;
		GameObject room = null;
		for (int i = 0; i < 9; i++)
		{
			newPos = currentPos + directions[i];
			try
			{
				//keep looping up through the parent transform until the parent is non existent or the parent is Map
				//should now work evene when rooms are childed to Map
				room = Physics2D.OverlapCircle(newPos, 1f).gameObject;
				while (room.transform.parent.name != "Map" || room.transform.parent == null)
				{
					room = transform.parent.gameObject;
				}
				//room = Physics2D.OverlapCircle(newPos, 1f).transform.root.gameObject;

				//ignore closed rooms, boss rooms, and exit rooms
				if (room != null && !room.name.Contains("C") && !room.name.Equals("BossRoom") && !room.name.Contains("Exit"))
				{
					rooms[rooms.ElementAt(i).Key] = room;
					//room.GetComponent<AddRoom>().extended = true;
				}
			}
			catch
			{
				//print("no room found - GetAdjacentRooms");
			}
			if (room == null)
			{
				continue;
			}
		}
		//to see if there are any active rooms, check for any FOCUS gameobject and if there is then get the parent room
		//check in the AddRoom component for the variable extended, if its false add to rooms, otherwise dont as it will have already been extended
		//for every room returned set the extended value in addroom to true so theyre not extended again
		return rooms;
	}

	public int CountEmptyRooms(Dictionary<string, GameObject> adjRooms)
	{
		int count = 0;
		foreach (GameObject room in adjRooms.Values)
		{
			if (room == null || room.name.Contains("C"))
			{
				count++;
			}
		}

		//print("The number of empty adjacent rooms is " + count);
		return count;
	}

	#region extension methods

	public void HorizontalExtend(Dictionary<string, GameObject> connectedRooms)
	{
		//current room is middle index
		//need rooms east and west
		try
		{ connectedRooms["BOTTOMLEFT"].GetComponent<AddRoom>().extended = false; }
		catch { }
		try
		{ connectedRooms["BOTTOM"].GetComponent<AddRoom>().extended = false; }
		catch { }
		try
		{ connectedRooms["BOTTOMRIGHT"].GetComponent<AddRoom>().extended = false; }
		catch { }

		GameObject[] rooms = new GameObject[6] { connectedRooms["TOPLEFT"], connectedRooms["TOP"], connectedRooms["TOPRIGHT"], connectedRooms["LEFT"], connectedRooms["CENTRE"], connectedRooms["RIGHT"] };

		DisableHorizontalWalls(rooms[0], rooms[1]);
		DisableHorizontalWalls(rooms[1], rooms[2]);

		//find if any rooms are horizontal to the currentroom
		//pass the valid rooms to the correct DisableWalls function
	}
	public void VerticalExtend(Dictionary<string, GameObject> connectedRooms)
	{
		try
		{ connectedRooms["TOPRIGHT"].GetComponent<AddRoom>().extended = false; }
		catch { }
		try
		{ connectedRooms["RIGHT"].GetComponent<AddRoom>().extended = false; }
		catch { }
		try
		{ connectedRooms["BOTTOMRIGHT"].GetComponent<AddRoom>().extended = false; }
		catch { }

		//need rooms north and south
		GameObject[] rooms = new GameObject[3] { connectedRooms["TOP"], connectedRooms["CENTRE"], connectedRooms["BOTTOM"] };

		DisableVerticalWalls(rooms[0], rooms[1]);
		DisableVerticalWalls(rooms[1], rooms[2]);
	}

	public void TwoxTwoRoom(Dictionary<string, GameObject> connectedRooms)
	{
		//connected rooms in top left, top, left, centre

		//disable rooms horizontally
		DisableHorizontalWalls(connectedRooms["TOPLEFT"], connectedRooms["TOP"]);
		DisableHorizontalWalls(connectedRooms["LEFT"], connectedRooms["CENTRE"]);

		//disable rooms vertically
		DisableVerticalWalls(connectedRooms["TOPLEFT"], connectedRooms["LEFT"]);
		DisableVerticalWalls(connectedRooms["TOP"], connectedRooms["CENTRE"]);
	}


	public void ThreexThreeRoom(Dictionary<string, GameObject> connectedRooms)
	{
		//need rooms in every direction including diagonally
		//disable horizontal walls
		//row 1
		DisableHorizontalWalls(connectedRooms["TOPLEFT"], connectedRooms["TOP"]);
		DisableHorizontalWalls(connectedRooms["TOP"], connectedRooms["TOPRIGHT"]);
		//row 2
		DisableHorizontalWalls(connectedRooms["LEFT"], connectedRooms["CENTRE"]);
		DisableHorizontalWalls(connectedRooms["CENTRE"], connectedRooms["RIGHT"]);
		//row 3
		DisableHorizontalWalls(connectedRooms["BOTTOMLEFT"], connectedRooms["BOTTOM"]);
		DisableHorizontalWalls(connectedRooms["BOTTOM"], connectedRooms["BOTTOMRIGHT"]);


		//disable vertical walls
		//column 1
		DisableVerticalWalls(connectedRooms["TOPLEFT"], connectedRooms["LEFT"]);
		DisableVerticalWalls(connectedRooms["LEFT"], connectedRooms["BOTTOMLEFT"]);
		//column 2
		DisableVerticalWalls(connectedRooms["TOP"], connectedRooms["CENTRE"]);
		DisableVerticalWalls(connectedRooms["CENTRE"], connectedRooms["BOTTOM"]);
		//column 3
		DisableVerticalWalls(connectedRooms["TOPRIGHT"], connectedRooms["RIGHT"]);
		DisableVerticalWalls(connectedRooms["RIGHT"], connectedRooms["BOTTOMRIGHT"]);
	}

	public void DisableVerticalWalls(GameObject a, GameObject b)
	{
		if (a == null || b == null)
		{
			return;
		}

		//disable the north walls between the 2 rooms
		//remove the walls enum from each room
		try
		{
			a.transform.Find("Walls").Find("South").gameObject.SetActive(false);
			b.transform.Find("Walls").Find("North").gameObject.SetActive(false);
		}
		catch
		{
			print(a.name);
			print(b.name);
		}

		a.GetComponent<AddRoom>().extended = true;
		b.GetComponent<AddRoom>().extended = true;
	}
	public void DisableHorizontalWalls(GameObject a, GameObject b)
	{
		if (a == null || b == null)
		{
			return;
		}

		//set the walls inactive
		a.transform.Find("Walls").Find("East").gameObject.SetActive(false);
		b.transform.Find("Walls").Find("West").gameObject.SetActive(false);

		//set the extended variable to true
		a.GetComponent<AddRoom>().extended = true;
		b.GetComponent<AddRoom>().extended = true;
	}

	#endregion

	#endregion
}
