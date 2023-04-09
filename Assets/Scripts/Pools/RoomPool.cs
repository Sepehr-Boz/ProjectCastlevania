using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Pools
{
	public class RoomPool : ObjectPooling
	{

		#region singleton
		private static RoomPool _instance;

		public static RoomPool Instance
		{
			get
			{
				return _instance;
			}
		}

		private void Awake()
		{
			_instance = this;
		}
		#endregion

		private new void Start()
		{
			//if (GameManager.Instance.thisArea.roomsData.Count == 0)
			//{
			//	////add the entry rooms to room data before adding the new instantiates room clones
			//	foreach (GameObject start in pooledObjects)
			//	{
			//		//add all walls when starting
			//		List<Wall> walls = new () {Wall.NORTH, Wall.EAST, Wall.SOUTH, Wall.WEST};

			//		GameManager.Instance.thisArea.roomsData.Add(new RoomData(start.transform.position, start.transform.rotation, walls, start.name, new List<GameObject>(), new List<GameObject>()));
			//		//set them inactive - RoomTemplates will either enable or keep them disabled
			//		start.SetActive(false);
			//	}
			//}

			//int n = amountToPool / objectsToPool.Length;
			//GameObject tmp;
			//foreach (GameObject room in objectsToPool)
			//{
			//	for (int i = 0; i < n; i++)
			//	{
			//		tmp = Instantiate(room);
			//		tmp.name = room.name;
			//		tmp.SetActive(false);


			//		pooledObjects.Add(tmp);
			//	}
			//}
		}

		public GameObject GetPooledRoom(string roomName = null)
		{
			GameObject tmp = null;

			foreach (GameObject room in objectsToPool)
			{
				if (room.name.Equals(roomName))
				{
					return room;
					//GameObject newRoom = Instantiate(room);
					//newRoom.name = roomName;

					//return newRoom;
				}
			}


			//foreach (GameObject room in pooledObjects)
			//{
			//	if (roomName.Equals(room.name) && !room.activeInHierarchy)
			//	{
			//		//room.transform.Find("SpawnPoints").gameObject.SetActive(true);
			//		return room;
			//	}
			//}

			print("room has not been found");
			print(roomName + " is the room tried to get");
			return tmp;
		}
	}


}