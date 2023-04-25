using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowTrap : TrapController
{
	private void OnTriggerEnter2D(Collider2D collision)
	{
		//check if collided with a damageable gameobject
		if (collision.GetComponent<IDamageable>() == null)
		{
			return;
		}

		//if pressed then play the particle effect at the collider location
		ParticleSystem newEffect = Instantiate(interactEffect);
		newEffect.transform.position = (Vector2)transform.position + GetComponent<Collider2D>().offset; //+offset moves the effect to where the collider would be
		newEffect.Play();

		//spawn the projectile and shoot it
		Invoke(nameof(Shoot), 0.25f);
	}
}
