using Assets.Scripts.MapGeneration;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Events;
using UnityEngine;
using System.Linq.Expressions;
using Unity.VisualScripting;
using System;

public class ExtensionMethods : MonoBehaviour
{
	private readonly Vector2[] directions =
	{
		new Vector2(-10, 10), new Vector2(0, 10), new Vector2(10, 10),
		new Vector2(-10, 0),  new Vector2(0, 0),  new Vector2(10, 0),
		new Vector2(-10, -10),new Vector2(0, -10),new Vector2(10, -10),
	};


	//public UnityEvent<Dictionary<string, GameObject>> extendFunction = new();
	public UnityEvent<Vector2> extendFunction = new();

	[Range(0, 100)]
	public int newEntryChance = 1;



	#region extending rooms

	public Dictionary<string, GameObject> GetAdjacentRooms(Vector2 currentPos)
	{
		//check if currentroom has already been extended and if it has then return
		//check for any rooms above, below, to the right, left, and diagonally of the current room
		Dictionary<string, GameObject> adjRooms = new(9)
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
				room = Physics2D.OverlapCircle(newPos, 1f, LayerMask.GetMask("Room")).gameObject;
				//print("room found is " + room.name);
			}
			catch
			{
				continue;
			}

			while (room.transform.parent != null && room.transform.parent.name != "Map")
			{
				room = room.transform.parent.gameObject;
			}

			if (room == null && !room.name.Contains("Boss") && !room.name.Contains("--") && !room.name.Contains("Exit"))
			{
				continue;
			}
			else
			{
				//print("ROOM NAME: " + room.name);
				adjRooms[adjRooms.ElementAt(i).Key] = room;
			}
		}
		return adjRooms;
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

	//public void HorizontalExtend(Dictionary<string, GameObject> connectedRooms)
	//{
	//	//current room is middle index
	//	//need rooms east and west
	//	try
	//	{ connectedRooms["BOTTOMLEFT"].GetComponent<AddRoom>().extended = false; }
	//	catch { }
	//	try
	//	{ connectedRooms["BOTTOM"].GetComponent<AddRoom>().extended = false; }
	//	catch { }
	//	try
	//	{ connectedRooms["BOTTOMRIGHT"].GetComponent<AddRoom>().extended = false; }
	//	catch { }

	//	GameObject[] rooms = new GameObject[6] { connectedRooms["TOPLEFT"], connectedRooms["TOP"], connectedRooms["TOPRIGHT"], connectedRooms["LEFT"], connectedRooms["CENTRE"], connectedRooms["RIGHT"] };

	//	DisableHorizontalWalls(rooms[0], rooms[1]);
	//	DisableHorizontalWalls(rooms[1], rooms[2]);

	//	//find if any rooms are horizontal to the currentroom
	//	//pass the valid rooms to the correct DisableWalls function
	//}
	//public void VerticalExtend(Dictionary<string, GameObject> connectedRooms)
	//{
	//	try
	//	{ connectedRooms["TOPRIGHT"].GetComponent<AddRoom>().extended = false; }
	//	catch { }
	//	try
	//	{ connectedRooms["RIGHT"].GetComponent<AddRoom>().extended = false; }
	//	catch { }
	//	try
	//	{ connectedRooms["BOTTOMRIGHT"].GetComponent<AddRoom>().extended = false; }
	//	catch { }

	//	//need rooms north and south
	//	GameObject[] rooms = new GameObject[3] { connectedRooms["TOP"], connectedRooms["CENTRE"], connectedRooms["BOTTOM"] };

	//	DisableVerticalWalls(rooms[0], rooms[1]);
	//	DisableVerticalWalls(rooms[1], rooms[2]);
	//}

	//public void TwoxTwoRoom(Dictionary<string, GameObject> connectedRooms)
	//{
	//	//connected rooms in top left, top, left, centre

	//	//disable rooms horizontally
	//	DisableHorizontalWalls(connectedRooms["TOPLEFT"], connectedRooms["TOP"]);
	//	DisableHorizontalWalls(connectedRooms["LEFT"], connectedRooms["CENTRE"]);

	//	//disable rooms vertically
	//	DisableVerticalWalls(connectedRooms["TOPLEFT"], connectedRooms["LEFT"]);
	//	DisableVerticalWalls(connectedRooms["TOP"], connectedRooms["CENTRE"]);
	//}


	//public void ThreexThreeRoom(Dictionary<string, GameObject> connectedRooms)
	//{
	//	//need rooms in every direction including diagonally
	//	//disable horizontal walls
	//	//row 1
	//	DisableHorizontalWalls(connectedRooms["TOPLEFT"], connectedRooms["TOP"]);
	//	DisableHorizontalWalls(connectedRooms["TOP"], connectedRooms["TOPRIGHT"]);
	//	//row 2
	//	DisableHorizontalWalls(connectedRooms["LEFT"], connectedRooms["CENTRE"]);
	//	DisableHorizontalWalls(connectedRooms["CENTRE"], connectedRooms["RIGHT"]);
	//	//row 3
	//	DisableHorizontalWalls(connectedRooms["BOTTOMLEFT"], connectedRooms["BOTTOM"]);
	//	DisableHorizontalWalls(connectedRooms["BOTTOM"], connectedRooms["BOTTOMRIGHT"]);


	//	//disable vertical walls
	//	//column 1
	//	DisableVerticalWalls(connectedRooms["TOPLEFT"], connectedRooms["LEFT"]);
	//	DisableVerticalWalls(connectedRooms["LEFT"], connectedRooms["BOTTOMLEFT"]);
	//	//column 2
	//	DisableVerticalWalls(connectedRooms["TOP"], connectedRooms["CENTRE"]);
	//	DisableVerticalWalls(connectedRooms["CENTRE"], connectedRooms["BOTTOM"]);
	//	//column 3
	//	DisableVerticalWalls(connectedRooms["TOPRIGHT"], connectedRooms["RIGHT"]);
	//	DisableVerticalWalls(connectedRooms["RIGHT"], connectedRooms["BOTTOMRIGHT"]);
	//}


	public void HorizontalExtend(Vector2 start)
	{
		//get all rooms across the y axis at the x at start
		start = RoundTo10s(start);

		//get a new array of points across the x axis
		List<Vector2> points = GetPoints(start, MODE.X);

		//loop through each point and find if theres a room at that location
		GameObject[] rooms = new GameObject[points.Count];
		for (int i = 0; i < rooms.Length; i++)
		{
			GameObject room = null;
			try
			{
				room = Physics2D.OverlapCircle(points[i], 1f, LayerMask.GetMask("Room")).gameObject;
				while (room.transform.parent != null && room.transform.parent.name != "Map")
				{
					room = room.transform.parent.gameObject;
				}
			}
			catch
			{
				rooms[i] = null;
				continue;
			}

			if ("UDLR".Contains(room.name))
			{
				rooms[i] = room;
			}
			else
			{
				rooms[i] = null;
			}
		}

		//loop through rooms and extend them horizontally
		for (int i = 0; i < rooms.Length - 1; i++)
		{
			DisableHorizontalWalls(rooms[i], rooms[i + 1]);
		}



		//at the end cancel the invoke of this method
		CancelInvoke(nameof(HorizontalExtend));
	}

	public void VerticalExtend(Vector2 start)
	{
		start = RoundTo10s(start);
		List<Vector2> points = GetPoints(start, MODE.Y);

		GameObject[] rooms = new GameObject[points.Count];
		for (int i = 0; i < rooms.Length; i++)
		{
			GameObject room = Physics2D.OverlapCircle(points[i], 1f, LayerMask.GetMask("Room")).gameObject;
			while (room.transform.parent != null && room.transform.parent.name != "Map")
			{
				room = room.transform.parent.gameObject;
			}

			if ("UDLR".Contains(room.name))
			{
				rooms[i] = room;
			}
			else
			{
				rooms[i] = null;
			}
		}

		//loop through rooms and extend them horizontally
		for (int i = 0; i < rooms.Length - 1; i++)
		{
			DisableVerticalWalls(rooms[i], rooms[i + 1]);
		}

		//stop invoking this method otherwise itll keep repeating and being invoked
		CancelInvoke(nameof(VerticalExtend));

	}

	public Vector2 RoundTo10s(Vector2 pos)
	{
		return new Vector2(Mathf.RoundToInt(pos.x / 10f) * 10, Mathf.RoundToInt(pos.y / 10f) * 10);
	}

	public List<Vector2> GetPoints(Vector2 start, MODE mode)
	{
		//make points a list as a list isnt fixed size but an array is
		List<Vector2> points = new();

		if (mode == MODE.X)
		{
			//get points across x axis
			for (int i = -5; i < 6; i++)
			{
				points.Add(new Vector2(start.x + (i * 10), start.y));
				//points[i] = new Vector2(start.x + i * 10, start.y);
				continue;
			}
		}
		else if (mode == MODE.Y)
		{
			//get points across y axis
			for (int i = 5; i > -6; i--)
			{
				points.Add(new Vector2(start.x, start.y + (i * 10)));
				//points[i] = new Vector2(start.x, start.y + i * 10);
			}
		}


		//foreach (Vector2 point in points){ print(point); };

		return points;
	}

	public enum MODE
	{
		X,
		Y
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

		//a.GetComponent<AddRoom>().extended = true;
		//b.GetComponent<AddRoom>().extended = true;
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
		//a.GetComponent<AddRoom>().extended = true;
		//b.GetComponent<AddRoom>().extended = true;
	}

	#endregion

	#endregion
}
