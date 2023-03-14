using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

namespace Assets.Scripts.Data
{
	[CreateAssetMenu(fileName = "AreaData")]
	public class AreaData : ScriptableObject
	{
		public Area area;
		public Color colour;

		public List<GameObject> enemies; //list of enemies that CAN BE SPAWNED //NOT THE ONES THAT HAVE BEEN SPAWNED
		public List<GameObject> objects; //list of objects that CAN BE SPAWNED //NOT THE ONES THAT HAVE BEEN SPAWNED

		public List<GameObject> rooms;
		[SerializeField]public List<RoomData> roomsData;

	}
}

public enum Area
{
	MAZEA,
	MAZEB,
	AREAA,
	AREAB,
	AREAC
}

//[System.Serializable] makes the struct visible in the inspector
[System.Serializable]public struct RoomData
{
	public Vector3 position;
	public Quaternion rotation;
	public string name;

	public List<GameObject> enemies; //enemies spawned in the room
	public List<GameObject> objects; //objects spawned in the room

	public RoomData(Vector3 pos, Quaternion rot, string name, List<GameObject> enemies, List<GameObject> objects)
	{
		this.position = pos;
		this.rotation = rot;
		this.name = name;

		this.enemies = enemies;
		this.objects = objects;
	}
	#region getters
	public Vector3 GetPosition()
	{
		return this.position;
	}
	public Quaternion GetRotation()
	{
		return this.rotation;
	}
	public string GetName()
	{
		return this.name;
	}
	public List<GameObject> GetEnemies()
	{
		return this.enemies;
	}
	public List<GameObject> GetObjects()
	{
		return this.objects;
	}
	#endregion

	#region setters

	public void AddEnemy(GameObject enemy)
	{
		this.enemies.Add(enemy);
	}


	#endregion
}