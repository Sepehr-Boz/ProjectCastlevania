using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroyer : MonoBehaviour {

	void OnTriggerEnter2D(Collider2D other){
		if (other.CompareTag("Player"))
		{
			//if the collider is the player then dont destroy
			return;
		}

		//Destroy(other.gameObject);
		Destroy(other.gameObject.GetComponent<RoomSpawner>());
		other.gameObject.SetActive(false);

	}
}
