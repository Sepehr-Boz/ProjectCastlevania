using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroyer : MonoBehaviour {

	void OnTriggerEnter2D(Collider2D other){
		print("destroyer collides with: " + other.name);

		if (other.CompareTag("Player") || other.name.Equals("FOCUS"))
		{
			//if the collider is the player then dont destroy
			return;
		}



		Destroy(other.gameObject);
	}
}
