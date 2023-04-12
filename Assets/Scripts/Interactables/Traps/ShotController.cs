using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotController : MonoBehaviour
{
    //may get warning message saying that collider hides inherited member Component.collider
    //what that means is that every gameobject already has a variable named 'collider' set to it that references the collider attached to the gameobject
    //HOWEVER, Component.collider is depreceated so can't use it anymore and have to define collider in script
    //have to user new Collider collider as the warning message will still appear even though its depreceated -_- bruh
    private new Collider2D collider;

    public int damage = 1;
    public float delay = 1f;

    private void Start()
    {
        collider = GetComponent<Collider2D>();

        //set the shoot inactive when not visible so it doesnt damage enemies when not in view
        if (!GetComponent<Renderer>().isVisible)
        {
            gameObject.SetActive(false);
        }
        else
        {
			StartCoroutine(EnableShot());
		}
    }

    private IEnumerator EnableShot()
    {
        collider.enabled = false;
        yield return new WaitForSeconds(delay);
        collider.enabled = true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerController>().hp -= damage;
        }

        //Destroy(gameObject);
        gameObject.SetActive(false);
    }
}
