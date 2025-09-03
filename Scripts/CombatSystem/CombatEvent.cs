using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatEvent : MonoBehaviour
{
    public IDamageAble Sender { get; set; }
    public IDamageAble Receiver { get; set; }
    public int Damage { get; set; }
    public Vector3 HitPosition;
    public Vector3 HitNormal;
    public Collider colider;
    public bool UseEffect;
    public Vector3 EffectPosition;
    public Vector3 EffectRotation;
    public string EffectName;
}