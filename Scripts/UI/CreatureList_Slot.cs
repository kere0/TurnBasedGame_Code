using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class CreatureList_Slot : MonoBehaviour, IPointerClickHandler
{
    public SaveCreatureInfo creatureInfo;
    private Image slotBackground; //
    private TextMeshProUGUI nameText; //
    private Image slotImage; //
    private GameObject starPanel; //
    private GameObject[] stars= new GameObject[3]; //
    private GameObject rateSlider; //
    private Image sliderBackground;
    private TextMeshProUGUI rateText;
    public int slotNum;
    
    private CreatureList_SlotController creatureList_SlotController;
    public bool hasData = false;
    public bool isSelected = false;
    private void Awake()
    {
        creatureList_SlotController = transform.parent.GetComponent<CreatureList_SlotController>();
        TryGetComponent(out slotBackground);
        nameText = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        slotImage = transform.GetChild(1).GetChild(0).GetComponent<Image>();
        starPanel = transform.GetChild(2).gameObject;
        for (int i = 0; i < stars.Length; i++)
        {
            stars[i] = starPanel.transform.GetChild(i).gameObject;
        }
        rateSlider = transform.GetChild(3).gameObject;
        sliderBackground = rateSlider.transform.GetChild(0).GetComponent<Image>();
        rateText = rateSlider.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
    }
    
    public void SetSlot()
    {
        nameText.text = creatureInfo.ID;
        if (creatureInfo.Rarity == Rarity.Normal)
        {
            slotBackground.sprite = Resources.Load<Sprite>($"Sprites/UI/SelectBackground_Gray");
            sliderBackground.sprite = Resources.Load<Sprite>($"Sprites/UI/Slider_White");
        }
        else if (creatureInfo.Rarity == Rarity.Epic)
        {
            slotBackground.sprite = Resources.Load<Sprite>($"Sprites/UI/SelectBackground_Blue");
            sliderBackground.sprite = Resources.Load<Sprite>($"Sprites/UI/Slider_Blue");
        }
        else if (creatureInfo.Rarity == Rarity.Legend)
        {
            slotBackground.sprite = Resources.Load<Sprite>($"Sprites/UI/SelectBackground_Yellow");
            sliderBackground.sprite = Resources.Load<Sprite>($"Sprites/UI/Slider_Yellow");
        }
        slotImage.sprite = Resources.Load<Sprite>($"Sprites/Creatures/{creatureInfo.ID}_{creatureInfo.Rarity}");
        slotImage.SetNativeSize();
        rateText.text = creatureInfo.Rarity.ToString();
        foreach (GameObject star in stars)
        {
            star.SetActive(false);
        }
        for (int i = 0; i < creatureInfo.Star; i++)
        {
            stars[i].SetActive(true);
        }
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        creatureList_SlotController.replaceText_UI.SetActive(true);
        creatureList_SlotController.currentSelectSlotNum = slotNum;
        //creatureList_SlotController.currentSelectNumId = creatureInfo.NumId;
    }
}
