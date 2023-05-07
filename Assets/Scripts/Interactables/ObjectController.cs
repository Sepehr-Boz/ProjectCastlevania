using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectController : MonoBehaviour
{
    [SerializeField] private Sprite active;
    [SerializeField] private Sprite destroyed;

    private void Start()
    {
       if (active != null)
        {
            GetComponent<SpriteRenderer>().sprite = active;
        }
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<IDamageable>() != null)
        {
            //add coins to the collider
            //GameManager.Instance.money++;
            //PlayerManager.Instance.currentData.money++;

            //change sprite to a destroyed one and disable collider
            if (destroyed != null)
            {
                GetComponent<SpriteRenderer>().sprite = destroyed;
            }
            GetComponent<Collider2D>().enabled = false;

            //spawn a coin
            GameObject coin = CoinPool.Instance.GetPooledObject();
            coin.transform.position = transform.position;
        }
    }
}
