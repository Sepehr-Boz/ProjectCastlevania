using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Data
{
	[CreateAssetMenu(fileName = "AreaData")]
	public class AreaData : ScriptableObject
	{
		public Color colour;

		[Header("Additionals")]
		[Space(3)]
		public List<EnemyData> enemies;
		public List<GameObject> objects; //list of objects that CAN BE SPAWNED //NOT THE ONES THAT HAVE BEEN SPAWNED

		[SerializeField]public List<RoomData> rooms;
		public int maxMapSize;
	}
}

public enum Wall
{
	NORTH,
	SOUTH,
	EAST,
	WEST
}

//[System.Serializable] makes the struct visible in the inspector
[System.Serializable]public struct RoomData
{
	public string name; //name of the room
	public Vector2 position; //rooms position in the scene
	public List<Wall> activeWalls; //walls that should be rendered

	public RoomData(Vector2 pos, string name, List<Wall> walls)
	{

		this.position = pos;
		this.activeWalls = walls;
		this.name = name;
	}

	public void RemoveInactiveWall(string wall)
	{
		if (wall == "North")
		{
			this.activeWalls.Remove(Wall.NORTH);
		}
		else if (wall == "East")
		{
			this.activeWalls.Remove(Wall.EAST);
		}
		else if (wall == "South")
		{
			this.activeWalls.Remove(Wall.SOUTH);
		}
		else if (wall == "West")
		{
			this.activeWalls.Remove(Wall.WEST);
		}
		else
		{
			Debug.Log("an error has occurred deleting walls");
		}
	}
}

[System.Serializable]public struct EnemyData
{
	public Vector2 position;
	public Quaternion rotation;

	public string name;
	public int hp;

	public GameObject enemyRef;

	public EnemyData(Vector2 pos, Quaternion rot, string enemyName, int currentHP, GameObject enemy)
	{
		this.position = pos;
		this.rotation = rot;
		this.name = enemyName;
		this.hp = currentHP;

		this.enemyRef = enemy;
	}
}




[System.Serializable]
public class ListNode<T>
{
	public T val; //T has to be specified in the class name, but when calling it the T has to specified
	public ListNode<T> next;

	public ListNode(T value, ListNode<T> next)
	{
		this.val = value;
		this.next = next;
	}
}