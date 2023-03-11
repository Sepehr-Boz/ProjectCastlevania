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

		//Destroy(other.gameObject);
		if (other.GetComponentInParent<AddRoom>().area != transform.GetComponentInParent<AddRoom>().area)
		{
			CompareRoomsB(other.transform.parent.parent.gameObject, transform.parent.parent.gameObject, other.GetComponent<RoomSpawner>().openingDirection);
		}


		Destroy(other.gameObject.GetComponent<RoomSpawner>());
		other.gameObject.SetActive(false);

	}

	private void CompareRoomsB(GameObject a, GameObject b, int dir)
	{
		Transform aWalls = a.transform.Find("Walls").transform;
		Transform bWalls = b.transform.Find("Walls").transform;

		if (dir == 1)
		{
			aWalls.Find("North").gameObject.SetActive(false);
			bWalls.Find("South").gameObject.SetActive(false);
		}
		else if (dir == 2)
		{
			aWalls.Find("South").gameObject.SetActive(false);
			bWalls.Find("North").gameObject.SetActive(false);
		}
		else if (dir == 3)
		{
			aWalls.Find("East").gameObject.SetActive(false);
			bWalls.Find("West").gameObject.SetActive(false);
		}
		else if (dir == 4)
		{
			aWalls.Find("West").gameObject.SetActive(false);
			bWalls.Find("East").gameObject.SetActive(false);
		}
		else
		{
			return;
		}
	}
}
