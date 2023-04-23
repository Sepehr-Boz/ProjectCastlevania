using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.MapGeneration
{
	public class PortalScript : MonoBehaviour
	{

		//enable self after a delay
		//this is to prevent the collision between boss coin and the portal after the boss has been defeated
		private void Start()
		{
			Invoke(nameof(EnableSelf), 3f);
		}

		private void EnableSelf()
		{
			GetComponent<Renderer>().enabled = true;
			GetComponent<Collider2D>().enabled = true;
		}


		private void OnTriggerEnter2D(Collider2D collision)
		{
			if (collision.CompareTag("Player"))
			{
				//update player stats
				PlayerManager.Instance.currentData.currentHP = PlayerManager.Instance.currentPlayer.GetComponent<PlayerController>().hp;


				//remake map
				GameManager.Instance.mapCreation.CreateMap();

				//do additional things like increase/decrease level
				GameManager.Instance.currentLevel++;

				//then destroy itself
				Destroy(gameObject);
			}
		}


	}
}
