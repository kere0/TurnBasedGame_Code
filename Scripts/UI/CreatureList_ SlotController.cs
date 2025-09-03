using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CreatureList_SlotController : MonoBehaviour
{
    public CreatureList_SlotController creatureList_SlotController;
    public GameObject replaceText_UI;
    private Button YesButton;
    private Button NoButton;
    private Button closeButton;
    List<CreatureList_Slot> creatureList_Slots = new List<CreatureList_Slot>();
    private GameObject slotPrefab;
    public int currentSelectSlotNum;
    private List<GameObject> slotClearList = new List<GameObject>();
    public Button button_Back;
    public GameObject creature_List;
    
    void Awake()
    {
        creatureList_SlotController = this;
    }
    void Start()
    {
        slotPrefab  = Resources.Load<GameObject>("CreatureList_Slot");
        YesButton = replaceText_UI.transform.GetChild(1).GetChild(1).GetComponent<Button>();        
        NoButton = replaceText_UI.transform.GetChild(1).GetChild(2).GetComponent<Button>();
        closeButton = replaceText_UI.transform.GetChild(1).GetChild(3).GetComponent<Button>();
        
        YesButton.onClick.AddListener(YesButton_OnClick);
        NoButton.onClick.AddListener(()=> replaceText_UI.SetActive(false));
        closeButton.onClick.AddListener(()=> replaceText_UI.SetActive(false));
        button_Back.onClick.AddListener(()=> creature_List.SetActive(false));
        CreatureList_SlotSet();
    }

    void CreatureList_SlotSet()
    {
        creatureList_Slots.Clear();
        foreach (var slot in slotClearList)
        {
            Destroy(slot);
        }
        slotClearList.Clear();
        int num = 0;
        foreach (var creatureInfo in Player.Instance.unCombatCreatureInfo)
        {
            GameObject go = Instantiate(slotPrefab, transform);
            slotClearList.Add(go);
            CreatureList_Slot slot = go.GetComponent<CreatureList_Slot>();
            slot.slotNum = num;
            num++;
            slot.creatureInfo = creatureInfo;
            creatureList_Slots.Add(slot);
            slot.SetSlot();
        }
    }
    void YesButton_OnClick()
    {
        replaceText_UI.SetActive(false);
        SaveCreatureInfo combatSaveCreatureInfo = Player.Instance.combatCreatureInfo[Slot_Controller.Instance.selectedSlotNum];

        SaveCreatureInfo combatCreatureInfo = new SaveCreatureInfo
        {
            ID = combatSaveCreatureInfo.ID,
            Star = combatSaveCreatureInfo.Star,
            Rarity = combatSaveCreatureInfo.Rarity,
            isUsing = false,
        };
        SaveCreatureInfo unCombatSaveCreatureInfo = Player.Instance.unCombatCreatureInfo[currentSelectSlotNum];
        SaveCreatureInfo unCombatCreatureInfo = new SaveCreatureInfo
        {
            ID = unCombatSaveCreatureInfo.ID,
            Star = unCombatSaveCreatureInfo.Star,
            Rarity = unCombatSaveCreatureInfo.Rarity,
            isUsing = true,
        };
        Player.Instance.combatCreatureInfo[Slot_Controller.Instance.selectedSlotNum] = unCombatCreatureInfo;
        Player.Instance.unCombatCreatureInfo[currentSelectSlotNum] = combatCreatureInfo;
        
        Slot_Controller.Instance.SetCreatureSlot();
        CreatureList_SlotSet();
        Player.Instance.CreatureSave_Firebase();
        Player.Instance.SetTotalCreature();
    }
}
