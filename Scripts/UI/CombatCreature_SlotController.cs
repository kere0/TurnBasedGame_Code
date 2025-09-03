using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatCreature_SlotController : MonoBehaviour
{
    public static CombatCreature_SlotController Instance;
    public CombatCreature_Slot[] combatCreatureSlot = new CombatCreature_Slot[3];
    void Awake()
    {
        Instance = this;
        for (int i = 0; i < combatCreatureSlot.Length; i++)
        {
            transform.GetChild(i).TryGetComponent(out combatCreatureSlot[i]);
        }
    }
    private void Start()
    {
        SetSlot();
    }
    void SetSlot()
    {
        int index = 0;
        foreach (SaveCreatureInfo creatureInfo in Player.Instance.combatCreatureInfo)
        {
            combatCreatureSlot[index].creatureImage.sprite = Resources.Load<Sprite>($"Sprites/Creatures/{creatureInfo.ID}_{creatureInfo.Rarity}");
            combatCreatureSlot[index].creatureImage.SetNativeSize();
            combatCreatureSlot[index].skillImage.sprite = Resources.Load<Sprite>($"Skills/Skill_Image/{creatureInfo.ID}_{creatureInfo.Rarity}");
            if (creatureInfo.Rarity != Rarity.Legend)
            {
                combatCreatureSlot[index].skillSlot.SetActive(false);
            }
            index++;
        }
    }
}