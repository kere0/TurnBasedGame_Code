using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Slot_Controller : MonoBehaviour
{
    public static Slot_Controller Instance;
    public Slot_Character[] slot_character = new Slot_Character[3];
    public Button button_Replace;
    public Button button_Back;
    public GameObject creatureList_UI;
    public GameObject canvas_CreatureSetting;
    
    public int selectedSlotNum = 0;

    public int currentSelectCreatureIdNum;
    void Awake()
    {
        Instance = this;
        button_Replace.onClick.AddListener(()=> creatureList_UI.SetActive(true));
        button_Back.onClick.AddListener(()=> canvas_CreatureSetting.SetActive(false));
        for (int i = 0; i < slot_character.Length; i++)
        {
            slot_character[i] = transform.GetChild(i).GetComponent<Slot_Character>();
        }
    }
    void Start()
    {
        SetCreatureSlot();
    }
    public void SetCreatureSlot()
    {
        for (int i = 0; i < Player.Instance.combatCreatureInfo.Count; i++)
        {
            slot_character[i].creatureInfo = Player.Instance.combatCreatureInfo[i];
            slot_character[i].SetSlot();
        }
    }
    public void SetFrameFocus(Slot_Character slotcharacter)
    {
        foreach (Slot_Character slot in slot_character)
        {
            if (slot == slotcharacter)
            {
                slot.frameFocus.SetActive(true);
                slot.isSelected = true;
                selectedSlotNum = slotcharacter.slotNum;
            }
            else
            {
                slot.frameFocus.SetActive(false);
                slot.isSelected = false;
            }
        }
    }
}
