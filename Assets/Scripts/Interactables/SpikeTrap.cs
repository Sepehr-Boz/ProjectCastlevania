using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeTrap : TrapController
{
    private SpriteRenderer spriteRenderer;

    [SerializeField] private float attackInterval;
    [SerializeField] private int damage;


    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        collider.enabled = false;

        StartCoroutine(Stab());
    }

    private IEnumerator Stab()
    {
        while (true)
        {
			collider.enabled = true;
            spriteRenderer.color = Color.red;
			yield return new WaitForSeconds(attackInterval);
			collider.enabled = false;
            spriteRenderer.color = Color.white;
            yield return new WaitForSeconds(attackInterval);
		}
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<PlayerController>().hp -= damage;
        }
    }
}
