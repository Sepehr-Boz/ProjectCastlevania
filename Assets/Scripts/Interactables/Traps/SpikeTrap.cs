using System.Collections;
using UnityEngine;

public class SpikeTrap : TrapController
{
    [SerializeField] private float attackInterval;
    [SerializeField] private int damage;


    private void Start()
    {
        GetComponent<Collider2D>().enabled = false;

        StartCoroutine(Stab());
    }

    private IEnumerator Stab()
    {
        yield return new WaitForSeconds(startDelay);


        while (true)
        {
			GetComponent<Collider2D>().enabled = true;
            GetComponent<SpriteRenderer>().color = Color.red;  //FOR TESTING
			yield return new WaitForSeconds(attackInterval);
			GetComponent<Collider2D>().enabled = false;
            GetComponent<SpriteRenderer>().color = Color.white; //FOR TESTING
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
