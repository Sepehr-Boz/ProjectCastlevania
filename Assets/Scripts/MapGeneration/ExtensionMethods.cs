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


	public UnityEvent<Vector2> extendFunction = new();


	#region getting rooms

	public Dictionary<string, GameObject> GetAdjacentRooms(Vector2 currentPos)
	{
		//check if currentroom has already been extended and if it has then return
		//check for any rooms above, below, to the right, left, and diagonally of the current room
		Dictionary<string, GameObject> adjRooms = new(9)
		{
			{"UPLEFT", null },  {"UP", null }, {"UPRIGHT", null },
			{"LEFT", null }, {"CENTRE", null }, {"RIGHT", null },
			{"DOWNLEFT", null }, {"DOWN", null }, {"DOWNRIGHT", null }
		};

		Vector2 newPos;
		GameObject room = null;
		for (int i = 0; i < 9; i++)
		{
			newPos = currentPos + directions[i];

			try
			{
				room = Physics2D.OverlapCircle(newPos, 3f, LayerMask.GetMask("Room")).gameObject;
			}
			catch
			{
				continue;
			}

			while (room.transform.parent != null && room.transform.parent.name != "Map")
			{
				room = room.transform.parent.gameObject;
			}

			adjRooms[adjRooms.ElementAt(i).Key] = room;
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
		return count;
	}

	#endregion

	#region extension methods

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
				continue;
			}
		}
		else if (mode == MODE.Y)
		{
			//get points across y axis
			for (int i = 5; i > -6; i--)
			{
				points.Add(new Vector2(start.x, start.y + (i * 10)));
				continue;
			}
		}

		return points;
	}

	public enum MODE
	{
		X,
		Y
	}



	public void DisableVerticalWalls(GameObject a, GameObject b)
	{
		if (a == null || a.name.Contains("--") || b == null || b.name.Contains("--"))
		{
			return;
		}

		//disable the UP walls between the 2 rooms
		//remove the walls enum from each room
		a.transform.Find("Walls").Find("DOWN").gameObject.SetActive(false);
		b.transform.Find("Walls").Find("UP").gameObject.SetActive(false);
	}
	public void DisableHorizontalWalls(GameObject a, GameObject b)
	{
		if (a == null || a.name.Contains("--") || b == null || b.name.Contains("--"))
		{
			return;
		}

		//set the walls inactive
		a.transform.Find("Walls").Find("RIGHT").gameObject.SetActive(false);
		b.transform.Find("Walls").Find("LEFT").gameObject.SetActive(false);
	}

	public void ExtendClosedRoom(GameObject room)
	{
		//get surrounding rooms
		var adjRooms = GetAdjacentRooms(room.transform.position);

		//extend vertically or horizontally if there are rooms in both directions
		if (adjRooms["UP"] && adjRooms["DOWN"])
		{
			DisableVerticalWalls(adjRooms["UP"], room);
			DisableVerticalWalls(room, adjRooms["DOWN"]);
			return;
		}
		else if (adjRooms["LEFT"] && adjRooms["RIGHT"])
		{
			DisableHorizontalWalls(adjRooms["LEFT"], room);
			DisableHorizontalWalls(room, adjRooms["RIGHT"]);
			return;
		}//otherwise only extend in one direction //also add extra cgeck statement so that it doesnt extend towards a corridor
		else if (adjRooms["UP"] && adjRooms["UP"].name.Contains("D") && !adjRooms["UP"].name.Contains("--"))
		{
			DisableVerticalWalls(adjRooms["UP"], room);
			return;
		}
		else if (adjRooms["DOWN"] && adjRooms["DOWN"].name.Contains("U") && !adjRooms["DOWN"].name.Contains("--"))
		{
			DisableVerticalWalls(room, adjRooms["DOWN"]);
			return;
		}
		else if (adjRooms["LEFT"] && adjRooms["LEFT"].name.Contains("R") && !adjRooms["LEFT"].name.Contains("--"))
		{
			DisableHorizontalWalls(adjRooms["LEFT"], room);
			return;
		}
		else if (adjRooms["RIGHT"] && adjRooms["RIGHT"].name.Contains("L") && !adjRooms["RIGHT"].name.Contains("--"))
		{
			DisableHorizontalWalls(room, adjRooms["RIGHT"]);
			return;
		}
		if (adjRooms["UP"])
		{
			DisableVerticalWalls(adjRooms["UP"], room);
		}
		if (adjRooms["DOWN"])
		{
			DisableVerticalWalls(room, adjRooms["DOWN"]);
		}
		if (adjRooms["LEFT"])
		{
			DisableHorizontalWalls(adjRooms["LEFT"], room);
		}
		if (adjRooms["RIGHT"])
		{
			DisableHorizontalWalls(room, adjRooms["RIGHT"]);
		}
	}

	#endregion
}