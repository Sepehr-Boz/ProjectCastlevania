using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject[] objects;


    private void Start()
    {
        //choose a random enemy from enemies and spawn it at the spawner location
        int rand = Random.Range(0, objects.Length);
        try
        {
			GameObject tmp = Instantiate(objects[rand]);
			tmp.SetActive(true);
			//child the enemy to the room so that if the room is destroyed then so is the enemy
			tmp.transform.parent = transform.parent;
			tmp.transform.position = transform.position;
		}catch{}
        //will catch an exception when trying to spawn an empty gameobject - as it should so that no enemy is spawned sometimes

        //destroy spawner gameobject as its not needed anymore
        Destroy(gameObject);
    }
}
