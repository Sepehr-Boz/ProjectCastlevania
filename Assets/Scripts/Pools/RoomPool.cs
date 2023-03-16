using JetBrains.Annotations;
using System;
using System.Collections;
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

		private void Start()
		{
			if (GameManager.Instance.thisArea.roomsData.Count == 0)
			{
				////add the entry rooms to room data before adding the new instantiates room clones
				foreach (GameObject start in pooledObjects)
				{
					//GameManager.Instance.thisArea.rooms.Add(start);
					//add all walls when starting
					Wall[] walls = new Wall[] { Wall.NORTH, Wall.SOUTH, Wall.EAST, Wall.EAST };
					GameManager.Instance.thisArea.roomsData.Add(new RoomData(start.transform.position, start.transform.rotation, walls, start.name, null, null));
					//set them inactive - RoomTemplates will either enable or keep them disabled
					start.SetActive(false);
				}

				//while (pooledObjects.Count > 0)
				//{
				//	GameManager.Instance.thisArea.roomsData.Add(new RoomData(pooledObjects[0].transform.position, pooledObjects[0].transform.rotation, pooledObjects[0].name, null, null));
				//	pooledObjects.RemoveAt(0);
				//}
			}

			base.Start();
		}

		public GameObject GetPooledRoom(string roomName = null)
		{
			GameObject tmp = null;

			foreach (GameObject room in pooledObjects)
			{
				if (roomName.Equals(room.name) && !room.activeInHierarchy)
				{
					//room.SetActive(true);
					room.transform.Find("SpawnPoints").gameObject.SetActive(true);
					return room;
				}
			}

			return tmp;
		}
	}


}