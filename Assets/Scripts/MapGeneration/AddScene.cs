using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class AddScene : MonoBehaviour
{
    private MapCreation mapCreation;

    public string scene;
    public Vector2 position;

    private void Start()
    {
        mapCreation = GameObject.FindGameObjectWithTag("Rooms").GetComponent<MapCreation>();
        //add random scene and position
        //get the first index scene from room templates and remove it from the list so it cant be another portal with the same scene to move to
        try
        {
			scene = mapCreation.moveToScenes[0].sceneName;
			position = mapCreation.moveToScenes[0].newPos;
			mapCreation.moveToScenes.RemoveAt(0);
		}
        catch
        {
			//destroy self if theres no scene
			///can sometimes occur when exit rooms are still added due to the difference in times between script runtimes allows multiple exit rooms to be spawned
			///even when theres only 1 scene name left etc. so this is just to check if theres no scene transition added to it and if there isnt then destroy itself
			///room name doesnt need to be changed nor does the RoomData equivalent as when spawning the map again this code will check again and see that theres no scene
			///so will destroy self again
			Destroy(gameObject);
        }

		//position = Vector2.zero;

		//print("scene to move to is " + scene);
    }


    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            //update the playerdata info
            PlayerManager.Instance.currentData.currentScene = scene;
            PlayerManager.Instance.currentData.currentPos = position;
            PlayerManager.Instance.currentData.currentHP = PlayerManager.Instance.currentPlayer.GetComponent<PlayerController>().hp;


            GameManager.Instance.ChangeScene(scene);
			//move the current player to position
			//PlayerManager.Instance.currentPlayer.transform.position = position;
		}
    }
}
