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
        scene = SceneManager.GetSceneByBuildIndex(Random.Range(0, SceneManager.sceneCountInBuildSettings)).name;
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
