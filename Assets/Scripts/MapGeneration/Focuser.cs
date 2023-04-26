using System.Collections;
using System.Collections.Generic;
using TreeEditor;
using UnityEngine;

public class Focuser : MonoBehaviour
{
	public GameObject room;

	public void OnTriggerEnter2D(Collider2D other)
	{
		//script is attached to checks in the checks parent
		//focus the camera to the room that this is attached to
		if (other.CompareTag("Player"))
		{
			//if the room hasnt been extended focus the camera to the room
			GameManager.Instance.virtualCamera.LookAt = transform;
			GameManager.Instance.virtualCamera.Follow = transform;


			//if player enters the room then enable it
			//ONLY THE PLAYER SHOULD ENABLE OR DISABLE ROOMS
			room.SetActive(true);
		}
	}

	public void OnTriggerExit2D(Collider2D other)
	{
		if (other.CompareTag("Player"))
		{
			room.SetActive(false);
		}
	}

}
