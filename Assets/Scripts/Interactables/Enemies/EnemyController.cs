using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class EnemyController : MonoBehaviour
{
    [Header("Movement")]
    [Range(0f, 1f)]
    [SerializeField]protected float moveSpeed;
    public Vector2 target;

    [Header("Combar")]
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

    protected void FixedUpdate()
    {
        if (target != null)
        {
            transform.position = Vector2.MoveTowards(transform.position, target, moveSpeed);
        }
    }


    protected void OnCollisionEnter2D(Collision2D collision)
    {
        //check if has IDamageable and if so make the gameobject take damage
    }

}
