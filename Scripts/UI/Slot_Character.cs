using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Slot_Character : MonoBehaviour, IPointerClickHandler
{
    //public CreatureData creatureData;
    public SaveCreatureInfo creatureInfo;
    public int slotNum;
    
    public bool hasData = false;
    public bool isSelected = false;
    private Image slotBackground;
    private TextMeshProUGUI nameText;
    private Image rateImage;
    private TextMeshProUGUI rateText;
    private Image characterImage;
    private GameObject starPanel;
    private GameObject[] stars= new GameObject[3];
    public  GameObject frameFocus;
    public GameObject buttonAdd;
    Color normalColor = new Color(1, 1, 1);
    Color epicColor = new Color(0, 0.35f, 0.87f);
    Color legendColor = new Color(1, 0.5f, 0.1f);

    void Awake()
    {
        TryGetComponent(out slotBackground);
        nameText = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        rateImage = transform.GetChild(1).GetComponent<Image>();
        rateText = rateImage.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        characterImage = transform.GetChild(2).GetComponent<Image>();
        starPanel = transform.GetChild(3).gameObject;
        for (int i = 0; i < stars.Length; i++)
        {
            stars[i] = starPanel.transform.GetChild(i).gameObject;
        }
    }
    public void SetSlot()
    {
        nameText.text = creatureInfo.ID;
        if (creatureInfo.Rarity == Rarity.Normal)
        {
            slotBackground.sprite = Resources.Load<Sprite>($"Sprites/UI/SelectBackground_Gray");
            nameText.color = normalColor;
            rateImage.sprite = Resources.Load<Sprite>($"Sprites/UI/Label_White");
        }
        else if (creatureInfo.Rarity == Rarity.Epic)
        {
            slotBackground.sprite = Resources.Load<Sprite>($"Sprites/UI/SelectBackground_Blue");
            nameText.color = epicColor;
            rateImage.sprite = Resources.Load<Sprite>($"Sprites/UI/Label_Blue");
        }
        else if (creatureInfo.Rarity == Rarity.Legend)
        {
            slotBackground.sprite = Resources.Load<Sprite>($"Sprites/UI/SelectBackground_Yellow");
            nameText.color = legendColor;
            rateImage.sprite = Resources.Load<Sprite>($"Sprites/UI/Label_Orange");
        }
        rateText.text = creatureInfo.Rarity.ToString();
        characterImage.sprite = Resources.Load<Sprite>($"Sprites/Creatures/{creatureInfo.ID}_{creatureInfo.Rarity}");
        characterImage.SetNativeSize();
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
        Slot_Controller.Instance.SetFrameFocus(this);
    }
}
