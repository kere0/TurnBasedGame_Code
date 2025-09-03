using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Creature_Info_UI : MonoBehaviour
{
    public static Creature_Info_UI Instance;
    CreatureData creatureData;
    private Image rankImage;//
    private  TextMeshProUGUI rankText;
    private TextMeshProUGUI nameText;//
    private Image slotImage; //
    private GameObject starPanel; //
    private GameObject[] stars= new GameObject[3]; //
    public TextMeshProUGUI attack_Text;
    public GameObject skillAttack;
    public TextMeshProUGUI skillAttack_Text;
    public TextMeshProUGUI hp_Text;
    public TextMeshProUGUI attackSpeed_Text;
    public Button button_Back;
    public GameObject upgrade_Canvas;
    public TextMeshProUGUI rune_Text;
    public Button upgrade_Button;
    public TextMeshProUGUI upgradeRune_Text;
    private int upgrade_RuneNum = 0;
    private int currentSlotNum;
    public GameObject skillBackGround;
    public Image skillImage;
    private void Awake()
    {
        Instance = this;
        button_Back.onClick.AddListener(()=> upgrade_Canvas.SetActive(false));
        rankImage = transform.GetChild(0).GetComponent<Image>();
        rankText = rankImage.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        nameText = transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        slotImage = transform.GetChild(2).GetComponent<Image>();
        starPanel = transform.GetChild(3).gameObject;
        for (int i = 0; i < stars.Length; i++)
        {
            stars[i] = starPanel.transform.GetChild(i).gameObject;
        }
        upgrade_Button.onClick.AddListener(UpgradeStar);
    }
    public void DataSet()
    {
        CreatureDataSet(0);
        rune_Text.text = Player.Instance.runeCount.ToString();
    }
    public void CreatureDataSet(int slotNum)
    {
        currentSlotNum = slotNum;
        SaveCreatureInfo saveCreatureInfo = Player.Instance.totalCreatureInfo[slotNum];
        CreatureData creatureData = Managers.CSVLoader.Get<CreatureData>(saveCreatureInfo.ID, saveCreatureInfo.Rarity, saveCreatureInfo.Star);
        nameText.text = creatureData.ID;
        attack_Text.text = creatureData.attack.ToString();
        skillAttack_Text.text = creatureData.skillAttack.ToString();
        hp_Text.text = creatureData.hp.ToString();
        attackSpeed_Text.text = creatureData.attackSpeed.ToString();
        rankText.text = creatureData.rarity.ToString();
        if (creatureData.hasSkill == false)
        {
            skillBackGround.SetActive(false);
            skillAttack.SetActive(false);
        }
        else
        {
            skillBackGround.SetActive(true);
            skillAttack.SetActive(true);
            skillImage.sprite = Resources.Load<Sprite>($"Skills/Skill_Image/{creatureData.ID}_{creatureData.Rarity}");
        }
        if (creatureData.Rarity == Rarity.Normal)
        {
            rankImage.sprite = Resources.Load<Sprite>($"Sprites/UI/Label_White");
        }
        else if (creatureData.Rarity == Rarity.Epic)
        {
            rankImage.sprite = Resources.Load<Sprite>($"Sprites/UI/Label_Blue");
        }
        else if (creatureData.Rarity == Rarity.Legend)
        {
            rankImage.sprite = Resources.Load<Sprite>($"Sprites/UI/Label_Orange");
        }
        slotImage.sprite = Resources.Load<Sprite>($"Sprites/Creatures/{creatureData.ID}_{creatureData.Rarity}");
        slotImage.SetNativeSize();
        foreach (GameObject star in stars)
        {
            star.SetActive(false);
        }
        for (int i = 0; i < creatureData.Star; i++)
        {
            stars[i].SetActive(true);
        }
        rune_Text.text = Player.Instance.runeCount.ToString();
        upgrade_RuneNum = creatureData.Star * 10;
        upgrade_RuneNum = Mathf.Clamp(upgrade_RuneNum, 0, 20);
        upgradeRune_Text.text = upgrade_RuneNum.ToString();
    }
    async void UpgradeStar()
    {
        if (Player.Instance.totalCreatureInfo[currentSlotNum].Star == 3) return;
        if (Player.Instance.runeCount >= upgrade_RuneNum)
        {
            Player.Instance.runeCount -= upgrade_RuneNum;
            if (Player.Instance.totalCreatureInfo[currentSlotNum].Star == 1)
            {
                Player.Instance.totalCreatureInfo[currentSlotNum].Star = 2;
            }
            else if (Player.Instance.totalCreatureInfo[currentSlotNum].Star == 2)
            {
                Player.Instance.totalCreatureInfo[currentSlotNum].Star = 3;
            }
            CreatureDataSet(currentSlotNum);
            CreatureList_UpgradeSlotController.Instance.CreatureList_SlotSet();
            await Player.Instance.CreatureSave_Firebase();
        }
    }
}
