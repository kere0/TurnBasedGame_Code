using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum Rarity
{
    Normal,
    Epic,
    Legend
}
public class CreatureData : InterfaceID
{
    public string id;
    public string name;
    public int star;
    public Rarity rarity;
    public int hp;
    public int attack;
    public int attackSpeed;
    public int skillAttack;
    public string skillName;
    public float skillCoolTime;
    public bool hasSkill;
    public string ID => id;
    public int Star => star;
    public Rarity Rarity => rarity;
}