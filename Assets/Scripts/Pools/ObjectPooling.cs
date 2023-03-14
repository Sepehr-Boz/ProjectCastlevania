using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Scripts.Pools
{
	public class ObjectPooling : MonoBehaviour
	{
		public List<GameObject> pooledObjects;
		public GameObject[] objectsToPool;

		public int amountToPool;

		public bool objectsPooled = false;

		void Start()
		{
			GameObject tmp;
			int n = objectsToPool.Length;

			pooledObjects = new List<GameObject>();
			//loop through amount to pool
			for (int i = 0; i < amountToPool; i++)
			{
				//pool objects
				tmp = Instantiate(objectsToPool[i % n]);
				tmp.transform.parent = transform;
				tmp.SetActive(false);
				pooledObjects.Add(tmp);
			}

			objectsPooled = true;


		}

		public GameObject GetPooledObject(GameObject objectName = null)
		{
			GameObject tmp;
			//check the pooled objects list for the wanted room and return it
			foreach (GameObject obj in pooledObjects)
			{
				if (objectName == obj && !obj.activeInHierarchy)
				{
					obj.SetActive(true);
					
					//tmp = obj;
					//RenewPooledObject(ref tmp);
					return obj;
				}
			}
			//if the room doesnt exist/ all instances are active already then make a new instance, add it to the list of pooled objects and return it
			tmp = Instantiate(objectName);
			pooledObjects.Add(tmp);
			tmp.transform.parent = transform;
			tmp.SetActive(true);
			//RenewPooledObject(ref tmp);
			return tmp;
		}

		//public void RenewPooledObject(ref GameObject objectName)
		//{
		//	if (objectName == null)
		//	{
		//		return;
		//	}

		//	Transform pointsParent = objectName.transform.Find("SpawnPoints").transform;

		//	foreach (Transform point in pointsParent)
		//	{
		//		point.gameObject.SetActive(true);

		//		if (point.gameObject.GetComponent<RoomSpawner>() != null || point.gameObject.GetComponent<Destroyer>() != null)
		//		{
		//			continue;
		//		}

		//		//add a roomspawner if one has been removed
		//		var spawner = point.AddComponent<RoomSpawner>();

		//		//add the appropiate direction to the script based on which direction the point is
		//		switch (point.name)
		//		{
		//			case "UP":
		//				spawner.openingDirection = 2;
		//				break;
		//			case "DOWN":
		//				spawner.openingDirection = 1;
		//				break;
		//			case "LEFT":
		//				spawner.openingDirection = 3;
		//				break;
		//			case "RIGHT":
		//				spawner.openingDirection = 4;
		//				break;
		//		}

		//		spawner.spawned = false;
		//		spawner.waitTime = 4;
		//	}
		//}
	}
}