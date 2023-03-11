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
			////if the room has been extended set the focus to the player
			//if (transform.parent.parent.GetComponent<AddRoom>().extended)
			//{
			//	PlayerManager.Instance.virtualCamera.LookAt = other.transform;
			//	PlayerManager.Instance.virtualCamera.Follow = other.transform;
			//	return;
			//}

			//if the room hasnt been extended focus the camera to the room
			PlayerManager.Instance.virtualCamera.LookAt = transform.parent;
			PlayerManager.Instance.virtualCamera.Follow = transform.parent;
		}
	}

}
