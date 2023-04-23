using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotController : MonoBehaviour
{
    //may get warning message saying that collider hides inherited member Component.collider
    //what that means is that every gameobject already has a variable named 'collider' set to it that references the collider attached to the gameobject
    //HOWEVER, Component.collider is depreceated so can't use it anymore and have to define collider in script
    //have to user new Collider collider as the warning message will still appear even though its depreceated -_- bruh
    //INSTEAD NOW DONT MAKE NEW REFERENCES TO ADDED COMPONENTS LIKE A RIGIDBODY OR COLLIDER
    //AS GetComponent<x> IS A REFERENCE ITSELF SO MAKING A NEW REFERENCE WILL JUST USE UP MORE MEMORY FOR NO REASON


    public int damage = 1;
    public float delay = 1f;

    private void Start()
    {
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

    private void Update()
    {
        if (!GetComponent<Renderer>().isVisible)
        {
            gameObject.SetActive(false);
        }
    }

    private IEnumerator EnableShot()
    {
        GetComponent<Collider2D>().enabled = false;
        yield return new WaitForSeconds(delay);
        GetComponent<Collider2D>().enabled = true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        try
        {
            collision.gameObject.GetComponent<IDamageable>().Damage(damage);
        }
        catch{}
        gameObject.SetActive(false);
    }
}
