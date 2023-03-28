using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroyer : MonoBehaviour {

	void OnTriggerEnter2D(Collider2D other){
		//print("destroyer collides with: " + other.name);

		if (other.CompareTag("Player") || other.name.Equals("FOCUS") || other.name.Equals("Boss") || other.name.Equals("Portal"))
		{
			//if the collider is the player then dont destroy
			return;
		}
		//if (other.name.Equals("CENTRE"))
		//{
		//	Destroy(other.transform.root.gameObject);
		//}

		//if (!other.name.Equals("CENTRE"))
		//{
		//	Destroy(other.gameObject);
		//}
		Destroy(other.gameObject);
	}
}
