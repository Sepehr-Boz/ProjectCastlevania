using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TrapController : MonoBehaviour
{
    [SerializeField] protected GameObject projectile;
    [SerializeField] protected Vector2 shootDir;
    [SerializeField] protected float shootSpeed;

    [SerializeField] protected ParticleSystem interactEffect;

    protected void Start()
    {
        if (!GetComponent<Collider2D>())
        {
            //if theres no collider add one
            gameObject.AddComponent<Collider2D>();
        }
    }

	protected void Shoot()
	{
		GameObject proj = Instantiate(projectile);
		proj.transform.position = transform.position;
		proj.SetActive(true);

		proj.GetComponent<Rigidbody2D>().velocity = shootDir * shootSpeed;
	}
}
