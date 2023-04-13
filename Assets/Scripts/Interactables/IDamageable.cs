using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    public int hp { get; set; }
    public int maxHP { get; set; }

    void Damage(int damage);
}
