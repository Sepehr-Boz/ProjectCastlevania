using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserTrap : TrapController
{
    [SerializeField] private float shootInterval;


    private void Start()
    {
        StartCoroutine(RepeatShooting()); //have shooting as a coruotine as an invokerepeating still invokes even when the gameobject is deactivated however a coruotine wont
    }

    private IEnumerator RepeatShooting()
    {
        yield return new WaitForSeconds(startDelay);
        while (true)
        {
            Shoot();
            yield return new WaitForSeconds(shootInterval);
        }
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
