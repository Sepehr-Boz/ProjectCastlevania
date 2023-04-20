using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroyer : MonoBehaviour {

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.CompareTag("SpawnPoint"))
		{
			Destroy(other.gameObject);
		}

		//if (GetRoom(other.transform).name.Contains("--") && other.name == "CENTRE")
		//{
		//	print("Collided with a corridor so remake new room");
		//	Time.timeScale = 0;
		//	GameManager.Instance.NewMap();
		//}
	}

	//private Transform GetRoom(Transform room)
	//{
	//	//room = transform;

	//	do
	//	{
	//		room = room.parent;
	//	}
	//	while (room.parent != null && room.parent.name != "Map");

	//	return room;
	//}
}
