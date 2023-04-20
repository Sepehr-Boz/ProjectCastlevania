using System.Collections;
using UnityEngine;

public class SpikeTrap : TrapController
{
    private new Collider2D collider;
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
            spriteRenderer.color = Color.red;  //FOR TESTING
			yield return new WaitForSeconds(attackInterval);
			collider.enabled = false;
            spriteRenderer.color = Color.white; //FOR TESTING
            yield return new WaitForSeconds(attackInterval);
		}
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        try
        {
            collision.GetComponent<IDamageable>().Damage(damage);
        }
        catch{}
    }
}
