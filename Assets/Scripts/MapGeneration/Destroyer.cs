using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroyer : MonoBehaviour {

	void OnTriggerEnter2D(Collider2D other){
		if (other.CompareTag("Player") || other.name == "FOCUS" || other.name == "Room")
		{
			//if the collider is the player then dont destroy
			return;
		}

		Destroy(other.gameObject);
	}
}
