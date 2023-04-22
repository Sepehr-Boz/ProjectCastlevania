using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserTrap : TrapController
{
    private new Collider2D collider;

    [SerializeField] private float shootInterval;


    private void Start()
    {
        collider = GetComponent<Collider2D>();

        InvokeRepeating(nameof(Shoot), startDelay, shootInterval);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //play hit effect
		ParticleSystem newEffect = Instantiate(interactEffect);
		newEffect.transform.position = transform.position;
		newEffect.Play();

        //delay shootinterval temporarily
        StartCoroutine(HaltShooting());
    }

	private IEnumerator HaltShooting()
    {
        CancelInvoke(nameof(Shoot));
        yield return new WaitForSeconds(5f);
        InvokeRepeating(nameof(Shoot), shootInterval, shootInterval);
    }
}
