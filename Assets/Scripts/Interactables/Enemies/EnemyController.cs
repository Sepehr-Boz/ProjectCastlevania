using Assets.Scripts.MapGeneration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.Interactables.Enemies
{
	public class EnemyController : HasHP, IDamageable
	{
		public float moveSpeed;

		public UnityEvent behaviour = new();
		[SerializeField] private float time = 5f;


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

				//invoke the CheckIfEnemiesNull method in the room
				//StartCoroutine(GetComponentInParent<AddRoom>().TriggerExits());
				GetComponentInParent<AddRoom>().TriggerExits();


				//if hp is less than or 0 destroy gameobject
				Destroy(gameObject);
			}

			time -= Time.deltaTime;
			if (time <= 0)
			{
				behaviour.Invoke();
				time = 5f;
			}
		}

		#region A
		public void MoveTowardsPlayer()
		{
			Vector2 target = LookForPlayer();
			if (target != (Vector2)transform.position)
			{
				GetComponent<Rigidbody2D>().velocity = Vector2.ClampMagnitude(target - (Vector2)transform.position, moveSpeed);
			}
		}
		#endregion


		#region B
		public void ShootPlayer()
		{
			Vector2 target = LookForPlayer();
			StartCoroutine(ShootXTimes(2, target));
		}

		private IEnumerator ShootXTimes(int num, Vector2 target)
		{
			//repeat num times
			for (int i = 0; i < num; i++)
			{
				//get projectile
				GameObject tmp = ProjectilePool.Instance.GetProjectile("Shot");
				tmp.transform.position = transform.position;
				tmp.GetComponent<Collider2D>().enabled = false;
				//tmp.GetComponent<Renderer>().enabled = false;
				tmp.SetActive(true);

				//add velocity towards target
				Vector2 move = target - (Vector2)transform.position;
				tmp.GetComponent<Rigidbody2D>().velocity = Vector2.ClampMagnitude(move, tmp.GetComponent<ShotController>().shootSpeed);

				yield return new WaitForSeconds(1f);
			}
		}
		#endregion

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

		public void Damage(int damage)
		{
			hp -= damage;
		}
	}
}
