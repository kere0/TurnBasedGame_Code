using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageAble
{
    public GameObject GameObject { get; }
    public Collider Collider { get; }
    public float Hp { get; }
    public bool IsDie { get; }
    public void TakeDamage(int damage, bool OnDamage);
}