using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinController : MonoBehaviour
{
    public int val = 0;

    private void OnEnable()
    {
        val = Random.Range(1, 20);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<PlayerController>())
        {
            //add value to coins
            GameManager.Instance.coins += val;

            Destroy(gameObject);
        }
    }
}
