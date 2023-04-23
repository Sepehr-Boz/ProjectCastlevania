using System.Collections;
using System.Collections.Generic;
using TMPro.EditorUtilities;
using UnityEngine;


[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class EnemyController : MonoBehaviour, IDamageable
{
    public int hp;
    public int maxHP;

    protected void Start()
    {
        hp = maxHP;
    }

    protected void Update()
    {
        if (hp <= 0)
        {
            //get and spawn coin from coinpool
            GameObject tmp = CoinPool.Instance.GetPooledObject();
            tmp.transform.position = transform.position;


            //if hp is less than or 0 destroy gameobject
            Destroy(gameObject);
        }
    }

    public void Damage(int damage)
    {
        hp -= damage;
    }

}
