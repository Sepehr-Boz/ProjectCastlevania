using System.Collections;
using System.Collections.Generic;
using TreeEditor;
using UnityEngine;

public class Focuser : MonoBehaviour
{
	public void OnTriggerEnter2D(Collider2D other)
	{
		//script is attached to checks in the checks parent
		//focus the camera to the room that this is attached to
		if (other.CompareTag("Player"))
		{
			//transform.root.gameObject.SetActive(true);
			//if the room hasnt been extended focus the camera to the room
			GameManager.Instance.virtualCamera.LookAt = transform.parent;
			GameManager.Instance.virtualCamera.Follow = transform.parent;
		}
	}

}
