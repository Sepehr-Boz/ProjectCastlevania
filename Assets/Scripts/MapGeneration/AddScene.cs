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
        //scene = SceneManager.GetSceneByBuildIndex(Random.Range(0, SceneManager.sceneCountInBuildSettings -1)).name;
        //var sceneNames = new List<string>() {"Area1", "Area2", "Area3", "Area4", "Area5", "Testing"};
        //scene = sceneNames[Random.Range(0, sceneNames.Count - 1)];

        //get the first index scene from room templates and remove it from the list so it cant be another portal with the same scene to move to
        scene = RoomTemplates.Instance.moveToScenes[0];
        RoomTemplates.Instance.moveToScenes.RemoveAt(0);
        //scene = GameManager.Instance.templates.moveToScenes[0];
        //GameManager.Instance.templates.moveToScenes.RemoveAt(0);

        print("scene to move to is " + scene);

        position = Vector2.zero;
    }


    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            GameManager.Instance.ChangeScene(scene, position);
            //move the current player to position
        }
    }
}
