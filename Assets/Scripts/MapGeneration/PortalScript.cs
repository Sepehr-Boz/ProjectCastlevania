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

		private void OnTriggerEnter2D(Collider2D collision)
		{
			if (collision.CompareTag("Player"))
			{
				//update player stats
				PlayerManager.Instance.currentData.currentHP = PlayerManager.Instance.currentPlayer.GetComponent<PlayerController>().hp;


				//remake map
				GameManager.Instance.mapCreation.CreateMap();

				//do additional things like increase/decrease level

				//then destroy itself
				Destroy(gameObject);
			}
		}


	}
}
