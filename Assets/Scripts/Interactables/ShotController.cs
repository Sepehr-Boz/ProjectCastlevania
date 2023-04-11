using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotController : MonoBehaviour
{
    private Collider2D collider;

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

        Destroy(gameObject);
    }
}
