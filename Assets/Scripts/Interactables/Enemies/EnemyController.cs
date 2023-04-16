using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class EnemyController : MonoBehaviour, IDamageable
{
    //int IDamageable.hp { get; set; }
    //int IDamageable.maxHP { get; set; }
    //public Vector2 hpVals;


    public int hp;
    //{
    //    get { return hp; }
    //    set { hp = value; }
    //}
    public int maxHP;
    //{
    //    get { return maxHP; }
    //    set { maxHP = value; }
    //}

	protected new Rigidbody2D rigidbody;
	protected new Collider2D collider;



	//[Header("Movement")]
 //   [Range(0f, 1f)]
 //   [SerializeField]protected float moveSpeed;
 //   public Vector2 target;

    protected void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        collider = GetComponent<Collider2D>();

        //hp = (int)hpVals.x;
        //maxHP = (int)hpVals.y;

        //print(hp + maxHP);

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

    //protected void FixedUpdate()
    //{
    //    if (target != null)
    //    {
    //        transform.position = Vector2.MoveTowards(transform.position, target, moveSpeed);
    //    }
    //}


    //protected void OnCollisionEnter2D(Collision2D collision)
    //{
    //    //check if has IDamageable and if so make the gameobject take damage
    //}

}
