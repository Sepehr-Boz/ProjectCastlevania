using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Interactables.Enemies
{
	public class EnemyController : MonoBehaviour
	{
		public int hp;
		public int maxHP;

		public float moveSpeed;


		protected void Start()
		{
			hp = maxHP;
		}


		protected void Update()
		{
			if (hp <= 0)
			{
				//get and spawn coin from coinpool
				GameObject tmp = CoinPool.Instance.GetPooledObject();
				tmp.transform.position = transform.position;


				//if hp is less than or 0 destroy gameobject
				Destroy(gameObject);
			}

			Vector2 target = LookForPlayer();
			if (target != (Vector2)transform.position)
			{
				GetComponent<Rigidbody2D>().velocity = Vector2.ClampMagnitude(target - (Vector2)transform.position, moveSpeed);
			}
		}

		private Vector2 LookForPlayer()
		{
			//look for if there exists a player inside the room
			Transform room = transform.parent;
			Collider2D[] overlap = Physics2D.OverlapBoxAll(room.position, new Vector2(5, 5), 0);

			foreach (Collider2D hit in overlap)
			{
				if (hit.gameObject.GetComponent<PlayerController>() != null)
				{
					return (Vector2)hit.transform.position;
				}
			}

			return (Vector2)transform.position;
		}


	}
}
