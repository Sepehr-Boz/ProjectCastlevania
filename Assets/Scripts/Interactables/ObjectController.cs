using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectController : MonoBehaviour
{
    [SerializeField] private int coins;

    [SerializeField] private Sprite active;
    [SerializeField] private Sprite destroyed;

    private new Collider2D collider;
    private new SpriteRenderer renderer;

    private void Start()
    {
        collider = GetComponent<Collider2D>();
        renderer = GetComponent<SpriteRenderer>();

        coins = Random.Range(1, 10);

       if (active != null)
        {
            renderer.sprite = active;
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
                renderer.sprite = destroyed;
            }
            collider.enabled = false;
        }
    }
}
