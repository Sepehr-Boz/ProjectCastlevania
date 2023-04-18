using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class EnemyController : MonoBehaviour, IDamageable
{
    public int hp;
    public int maxHP;

	protected new Rigidbody2D rigidbody;
	protected new Collider2D collider;


    protected void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        collider = GetComponent<Collider2D>();

        hp = maxHP;
    }

    protected void Update()
    {
        if (hp <= 0)
        {
            //if hp is less than or 0 destroy gameobject and also remove the enemydata from the enemies list
            try
            {
                //only destroy the enemy reference if theres an AddEnemy added to the object
				GameManager.Instance.thisArea.enemies.Remove(GetComponent<AddEnemy>().dataRef);
			}
            catch{}
            Destroy(gameObject);
        }
    }

    public void Damage(int damage)
    {
        hp -= damage;
    }

}
