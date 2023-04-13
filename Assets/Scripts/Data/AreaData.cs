using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Data
{
	[CreateAssetMenu(fileName = "AreaData")]
	public class AreaData : ScriptableObject
	{
		public Area area;
		public Color colour;

		[Header("Additionals")]
		[Space(3)]
		public List<EnemyData> enemies;
		//public List<GameObject> enemies; //list of enemies that CAN BE SPAWNED //NOT THE ONES THAT HAVE BEEN SPAWNED
		public List<GameObject> objects; //list of objects that CAN BE SPAWNED //NOT THE ONES THAT HAVE BEEN SPAWNED

		[Header("Rooms")]
		[Space(3)]
		public List<GameObject> rooms;
		[SerializeField]public List<RoomData> roomsData;

		public int maxMapSize;
	}
}

public enum Area
{
	AREA1,
	AREA2,
	AREA3,
	AREA4,
	AREA5,

	TESTING
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
	//public Quaternion rotation;
	public List<Wall> activeWalls; //walls that should be rendered

	//public string name; //name of the room

	//public List<GameObject> enemies; //enemies present in the room
	//public List<GameObject> objects; //objects present in the room

	//public Dictionary<string, int> exit; //the scene that should be loaded whenever exited the room

	//public RoomData(Vector3 pos, Quaternion rot, List<Wall> walls, string name, List<GameObject> enemies, List<GameObject> objects)
	public RoomData(Vector2 pos, string name, List<Wall> walls)
	{

		this.position = pos;
		//this.rotation = rot;
		this.activeWalls = walls;
		this.name = name;

		//this.enemies = enemies;
		//this.objects = objects;

		//this.exit = new Dictionary<string, int>(1);
	}

	//variables are public so getters and setters arent needed
	//#region getters
	//public Vector3 GetPosition()
	//{
	//	return this.position;
	//}
	//public Quaternion GetRotation()
	//{
	//	return this.rotation;
	//}
	//public List<Wall> GetActiveWalls()
	//{
	//	return this.activeWalls;
	//}
	//public string GetName()
	//{
	//	return this.name;
	//}
	//public List<GameObject> GetEnemies()
	//{
	//	return this.enemies;
	//}
	//public List<GameObject> GetObjects()
	//{
	//	return this.objects;
	//}
	//#endregion

	//#region setters

	//public void AddEnemy(GameObject enemy)
	//{
	//	this.enemies.Add(enemy);
	//}

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

	//public void SetExit(string sceneName, int index)
	//{
	//	this.exit.Add(sceneName, index);
	//}

	//public void Rename(string newName)
	//{
	//	this.name = newName;
	//}

	//#endregion
}

[System.Serializable]public struct EnemyData
{
	public Vector3 position;
	public Quaternion rotation;

	public string name;
	public int hp;

	public EnemyData(Vector3 pos, Quaternion rot, string enemyName, int currentHP)
	{
		this.position = pos;
		this.rotation = rot;
		this.name = enemyName;
		this.hp = currentHP;
	}
}




[System.Serializable]
public class ListNode<T>
{
	public T val; //T has to be specified in the class name, but when calling it the T has to specified
				  //public Vector2 position;
	public ListNode<T> next;
	//public TargetNode next;

	public ListNode(T value, ListNode<T> next)
	{
		this.val = value;
		this.next = next;
	}
}