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
				//add the entry rooms to room data before adding the new instantiates room clones
				foreach (GameObject start in pooledObjects)
				{
					//GameManager.Instance.thisArea.rooms.Add(start);
					GameManager.Instance.thisArea.roomsData.Add(new RoomData(start.transform.position, start.transform.rotation, start.name, null, null));
					//set them inactive - RoomTemplates will either enable or keep them disabled
					start.SetActive(false);
				}
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
					return room;
				}
			}

			return tmp;
		}
	}


}