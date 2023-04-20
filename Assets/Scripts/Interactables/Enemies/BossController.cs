using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Interactables.Enemies
{
	public class BossController : EnemyController
	{
		public GameObject portal;


		private void FixedUpdate()
		{
			if (hp <= 0)
			{
				//spawn portal
				GameObject tmp = Instantiate(portal);
				tmp.transform.position = transform.parent.position;


				//destroy self
				Destroy(gameObject);
			}
		}
	}
}
