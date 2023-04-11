using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TrapController : MonoBehaviour
{
    protected Collider2D collider;

    [SerializeField] protected GameObject projectile;
    [SerializeField] protected Vector2 shootDir;
    [SerializeField] protected float shootSpeed;

    [SerializeField] protected ParticleSystem interactEffect;

    protected void Start()
    {
        collider = GetComponent<Collider2D>();
    }

	protected void Shoot()
	{
		GameObject proj = Instantiate(projectile);
		proj.transform.position = (Vector2)transform.position + shootDir * 2f;
		proj.SetActive(true);

		proj.GetComponent<Rigidbody2D>().velocity = shootDir * shootSpeed;
	}
}
