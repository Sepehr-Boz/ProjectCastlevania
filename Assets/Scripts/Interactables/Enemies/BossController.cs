using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Interactables.Enemies
{
	public class BossController : EnemyController
	{
		public GameObject portal;

		//private new void Start()
		//{
		//	base.Start();

		//	//coins = Random.Range(15, 50);
		//}


		private new void Update()
		{
			if (hp <= 0)
			{
				//spawn portal after a delay to allow the coins to be picked up first
				GameObject tmp = Instantiate(portal);
				tmp.GetComponent<Renderer>().enabled = false;
				tmp.GetComponent<Collider2D>().enabled = false;
			}

			base.Update();
		}
	}
}
