using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject[] enemies;


    private void Start()
    {
        //choose a random enemy from enemies and spawn it at the spawner location
        int rand = Random.Range(0, enemies.Length);
        GameObject tmp = Instantiate(enemies[rand]);
        //child the enemy to the room so that if the room is destroyed then so is the enemy
        tmp.transform.parent = transform.parent;
        tmp.transform.position = transform.position;

        //destroy spawner gameobject as its not needed anymore
        Destroy(gameObject);
    }
}
