using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Bottom_UIController : MonoBehaviour
{
    private GameObject upgrade_Button;
    private GameObject card_Button;
    public GameObject creatureSetting;
    public GameObject upgrade_Canvas;
    public GameObject dungeon_Button;
    public GameObject dungeon_Canvas;
    public GameObject pvp_Canvas;
    public GameObject pvp_Button;
    public GameObject ranking_Canvas;
    public GameObject ranking_Button;
    void Awake()
    {
        upgrade_Button = transform.GetChild(0).GetChild(0).gameObject;
        card_Button = transform.GetChild(0).GetChild(1).gameObject;
        dungeon_Button = transform.GetChild(1).gameObject;
        card_Button.GetComponent<Button>().onClick.AddListener(CreatureSetting);
        upgrade_Button.GetComponent<Button>().onClick.AddListener(UpgradeUISet);
        dungeon_Button.GetComponent<Button>().onClick.AddListener(() => dungeon_Canvas.SetActive(true));
        pvp_Button.GetComponent<Button>().onClick.AddListener(PVPCanvasClick);
        ranking_Button.GetComponent<Button>().onClick.AddListener(() => ranking_Canvas.SetActive(true));
    }

    void CreatureSetting()
    {
        creatureSetting.SetActive(true);
        Slot_Controller.Instance.SetCreatureSlot();
    }
    void UpgradeUISet()
    {
        upgrade_Canvas.SetActive(true);
        Creature_Info_UI.Instance.DataSet();
    }

    void PVPCanvasClick()
    {
        pvp_Canvas.SetActive(true);
        PVPCreature_SlotController.Instance.SetCreatureInfo();
    }
}
