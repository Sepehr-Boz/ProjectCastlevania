using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroyer : MonoBehaviour {

	void OnTriggerEnter2D(Collider2D other)
	{
		//destory the SpawnPoints parent after 3 seconds
		Destroy(transform.parent.gameObject, 5f);


		if (other.CompareTag("SpawnPoint"))
		{
			Destroy(other.gameObject);
		}
	}
}
