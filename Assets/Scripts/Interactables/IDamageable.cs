using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SerializeField]
public interface IDamageable
{
    void Damage(int damage);
}
