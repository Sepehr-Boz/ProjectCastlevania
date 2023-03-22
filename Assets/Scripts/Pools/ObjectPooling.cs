﻿using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Scripts.Pools
{
	public abstract class ObjectPooling : MonoBehaviour
	{
		public List<GameObject> pooledObjects;
		public GameObject[] objectsToPool;

		public int amountToPool;

		public bool objectsPooled = false;

		public void Start()
		{
			GameObject tmp;
			int n = objectsToPool.Length;

			pooledObjects = new List<GameObject>();
			//loop through amount to pool
			for (int i = 0; i < amountToPool; i++)
			{
				//pool objects
				tmp = Instantiate(objectsToPool[i % n]);
				//sets the name to not have "(Clone)"
				tmp.name = objectsToPool[i % n].name;
				//tmp.transform.parent = transform;
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
					return obj;
				}
			}
			//if the room doesnt exist/ all instances are active already then make a new instance, add it to the list of pooled objects and return it
			tmp = Instantiate(objectName);
			pooledObjects.Add(tmp);
			tmp.transform.parent = transform;
			tmp.SetActive(true);
			return tmp;
		}
	}
}