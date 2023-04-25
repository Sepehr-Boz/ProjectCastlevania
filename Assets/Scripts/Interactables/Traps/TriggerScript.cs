using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerScript : MonoBehaviour
{
    public GameObject[] traps;

	[SerializeField] private ParticleSystem interactEffect;


	private void OnTriggerEnter2D(Collider2D collision)
    {
		if (collision.GetComponent<IDamageable>() == null)
		{
			return;
		}

		//if pressed then play the particle effect at the collider location
		ParticleSystem newEffect = Instantiate(interactEffect);
		newEffect.transform.position = (Vector2)transform.position + GetComponent<Collider2D>().offset; //+offset moves the effect to where the collider would be
		newEffect.Play();

		//loop through all the attachs traps and trigger their shoot function
		foreach (GameObject trap in traps)
		{
			trap.GetComponent<ArrowTrap>().Shoot();
		}
	}
}
