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

	}
}