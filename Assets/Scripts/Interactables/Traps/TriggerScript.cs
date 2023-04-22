using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerScript : MonoBehaviour
{
    public GameObject[] traps;

	[SerializeField] private ParticleSystem interactEffect;



	private new Collider2D collider;

	private void Start()
	{
		collider = GetComponent<Collider2D>();
	}


	private void OnTriggerEnter2D(Collider2D collision)
    {
		//if pressed then play the particle effect at the collider location
		ParticleSystem newEffect = Instantiate(interactEffect);
		newEffect.transform.position = (Vector2)transform.position + collider.offset; //+offset moves the effect to where the collider would be
		newEffect.Play();

		//loop through all the attachs traps and trigger their shoot function
		foreach (GameObject trap in traps)
		{
			trap.GetComponent<ArrowTrap>().Shoot();
		}
	}
}
