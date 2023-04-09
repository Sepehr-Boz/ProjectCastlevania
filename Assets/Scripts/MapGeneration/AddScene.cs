using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class AddScene : MonoBehaviour
{
    public string scene;
    public Vector2 position;

    private void Start()
    {
        //add random scene and position
        //get the first index scene from room templates and remove it from the list so it cant be another portal with the same scene to move to
        scene = RoomTemplates.Instance.moveToScenes[0];
        RoomTemplates.Instance.moveToScenes.RemoveAt(0);

        if (scene == null || scene == "")
        {
            //change name to not have exit
            transform.root.name.Replace("Exit", "");
            //destroy self as the portal wont lead anywhere
            Destroy(gameObject);
        }

		position = Vector2.zero;

		print("scene to move to is " + scene);
    }


    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            GameManager.Instance.ChangeScene(scene);
			//move the current player to position
			PlayerManager.Instance.currentPlayer.transform.position = position;
		}
    }
}
