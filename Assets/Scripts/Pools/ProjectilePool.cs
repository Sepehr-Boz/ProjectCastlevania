using Assets.Scripts.Pools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectilePool : ObjectPooling
{
	//used for all projectiles, player, and enemies

	#region singleton
	private static ProjectilePool _instance;

	public static ProjectilePool Instance
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
		base.Start();

		foreach (GameObject proj in pooledObjects)
		{
			proj.transform.parent = transform;
		}
	}

	public GameObject GetProjectile(string projName)
	{
		foreach (GameObject proj in pooledObjects)
		{
			if (proj.name.Equals(projName) && !proj.activeInHierarchy)
			{
				return proj;
			}
		}

		return null;
	}
}
