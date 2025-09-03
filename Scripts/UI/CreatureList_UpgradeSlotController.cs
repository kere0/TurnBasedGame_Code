using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CreatureList_UpgradeSlotController : MonoBehaviour
{
    public static CreatureList_UpgradeSlotController Instance;
    List<CreatureList_UpgradeSlot> creatureList_Slots = new List<CreatureList_UpgradeSlot>();
    private GameObject slotPrefab;
    private List<GameObject> slotClearList = new List<GameObject>();
    public int currentSlotNum = 0;

    void Awake()
    {
        Instance = this;
    }
    void OnEnable()
    {
        slotPrefab  = Resources.Load<GameObject>("CreatureList_UpgradeSlot");
        CreatureList_SlotSet();
    }
    public void CreatureList_SlotSet()
    {
        creatureList_Slots.Clear();
        foreach (var slot in slotClearList)
        {
            Destroy(slot);
        }
        slotClearList.Clear();
        int num = 0;
        foreach (var creatureInfo in Player.Instance.totalCreatureInfo)
        {
            GameObject go = Instantiate(slotPrefab, transform);
            slotClearList.Add(go);
            CreatureList_UpgradeSlot slot = go.GetComponent<CreatureList_UpgradeSlot>();
            slot.slotNum = num;
            num++;
            slot.creatureInfo = creatureInfo;
            creatureList_Slots.Add(slot);
            slot.SetSlot();
        }
        creatureList_Slots[currentSlotNum].frameFocus.SetActive(true);
    }
    public void SetFrameFocus(CreatureList_UpgradeSlot slotcharacter)
    {
        foreach (CreatureList_UpgradeSlot slot in creatureList_Slots)
        {
            if (slot == slotcharacter)
            {
                slot.frameFocus.SetActive(true);
                slot.isSelected = true;
            }
            else
            {
                slot.frameFocus.SetActive(false);
                slot.isSelected = false;
            }
        }
    }
}
