//using System;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.SceneManagement;
//using Random = UnityEngine.Random;

//public class AddScene : MonoBehaviour
//{
//    private MapCreation mapCreation;

//    public string scene;
//    public Vector2 position;

//    private void Start()
//    {
//        mapCreation = GameObject.FindGameObjectWithTag("Rooms").GetComponent<MapCreation>();
//        //add random scene and position
//        //get the first index scene from room templates and remove it from the list so it cant be another portal with the same scene to move to
//        try
//        {
//            //destroy the portal if theres no scene to get
//            scene = mapCreation.moveToScenes[0].sceneName;
//            position = mapCreation.moveToScenes[0].newPos;
//            mapCreation.moveToScenes.RemoveAt(0);
//        }
//        catch
//        {
//            Destroy(gameObject);
//        }
//    }


//    public void OnCollisionEnter2D(Collision2D collision)
//    {
//        if (collision.collider.CompareTag("Player"))
//        {
//            //update the playerdata info
//            //PlayerManager.Instance.currentData.currentScene = scene;
//            //PlayerManager.Instance.currentData.currentPos = position;
//            PlayerManager.Instance.currentData.currentHP = PlayerManager.Instance.currentPlayer.GetComponent<PlayerController>().hp;

//            GameManager.Instance.ChangeScene(scene);
//		}
//    }
//}
