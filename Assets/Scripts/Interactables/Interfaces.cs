using UnityEngine;

[SerializeField]
public abstract class HasHP: MonoBehaviour
{
    public int hp;
    public int maxHP;
}

public interface IDamageable
{
    void Damage(int damage);
}


public interface IIndestructable
{
}