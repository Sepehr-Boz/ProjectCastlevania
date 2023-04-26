using Assets.Scripts.Interactables.Enemies;
using Assets.Scripts.MapGeneration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class Bouncer : HasHP, IDamageable
{
    [SerializeField] private Vector2 dir;

    //tried to make dirs const but in C# const arrays aren't possible. Readonly should deliver the same function I want however.
    private readonly Vector2[] dirs = new Vector2[4]
    {
        new Vector2(0, 1),
		new Vector2(1, 0),
		new Vector2(0, -1),
		new Vector2(-1, 0)
	};


    // Start is called before the first frame update
    private void Start()
    {
        hp = maxHP;

        dir = dir == Vector2.zero ? new Vector2(Random.Range(-1, 1), Random.Range(-1, 1)) : dir;
    }

    private void FixedUpdate()
    {
        transform.position += (Vector3)dir / 35f;
    }

    private void Update()
    {
		if (hp <= 0)
		{
			//get and spawn coin from coinpool
			GameObject tmp = CoinPool.Instance.GetPooledObject();
			tmp.transform.position = transform.position;

            //StartCoroutine(GetComponentInParent<AddRoom>().TriggerExits());
            GetComponentInParent<AddRoom>().TriggerExits();


			//if hp is less than or 0 destroy gameobject
			Destroy(gameObject);
		}
	}

    //collisions between 2 objects NEED at least one of the objects to have a rigidbody otherwise the collision wont trigger
    private void OnCollisionEnter2D(Collision2D collision)
    {
        //loop through the directions of the raycasts
        foreach (Vector2 castDir in dirs)
        {
            try
            {
                //get the second collider from a raycast from centre outwards
                //get second hit as first will be the gameobject itself
                RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, castDir, 0.6f);

                foreach (RaycastHit2D hit in hits)
                {
					//check that it something thats valid
					if (hit.transform.root.GetComponent<IIndestructable>() != null || hit.transform.GetComponent<IDamageable>() != null)
					{
						//draw a line to see where it hits - FOR TESTING
						Debug.DrawLine(transform.position, hit.point, Color.red, 5f);

						//if hit then decrement the dir the gameobject moves in by castDir to make it 'bounce'/'reflect'
						//dir -= castDir;
                        if (castDir.x != 0)
                        {
                            dir.x = -dir.x;
                        }
                        else if (castDir.y != 0)
                        {
                            dir.y = -dir.y;
                        }
					}
				}
            }
            catch
            {
                //will be caught if theres no collision other than self so continue onwards
                continue;
            }
        }
	}

    public void Damage(int damage)
    {
        hp -= damage;
    }
}
