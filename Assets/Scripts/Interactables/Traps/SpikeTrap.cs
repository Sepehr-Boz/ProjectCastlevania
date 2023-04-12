using System.Collections;
using UnityEngine;

public class SpikeTrap : TrapController
{
    private SpriteRenderer spriteRenderer;

    [SerializeField] private float attackInterval;
    [SerializeField] private int damage;


    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        collider = GetComponent<Collider2D>();

        collider.enabled = false;

        StartCoroutine(Stab());
    }

    private IEnumerator Stab()
    {
        yield return new WaitForSeconds(startDelay);


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
