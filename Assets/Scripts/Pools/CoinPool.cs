using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Pools;

public class CoinPool : ObjectPooling
{
	#region singleton

	private static CoinPool _instance;
	public static CoinPool Instance
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
}
