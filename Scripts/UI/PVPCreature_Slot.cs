using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PVPCreature_Slot : MonoBehaviour
{
    public SaveCreatureInfo creatureInfo;

    public Image creatureImage;
    public Image backgroundImage;
    public GameObject starPanel;
    private GameObject[] stars= new GameObject[3];
    
    void Awake()
    {
        for (int i = 0; i < stars.Length; i++)
        {
            stars[i] = starPanel.transform.GetChild(i).gameObject;
        }
    }
    public void SetSlot()
    {
        creatureImage.sprite = Resources.Load<Sprite>($"Sprites/Creatures/{creatureInfo.ID}_{creatureInfo.Rarity}");
        creatureImage.SetNativeSize();
        if (creatureInfo.Rarity == Rarity.Normal)
        {
            backgroundImage.sprite = Resources.Load<Sprite>($"Sprites/UI/SelectBackground_Gray");
        }
        else if (creatureInfo.Rarity == Rarity.Epic)
        {
            backgroundImage.sprite = Resources.Load<Sprite>($"Sprites/UI/SelectBackground_Blue");
        }
        else if (creatureInfo.Rarity == Rarity.Legend)
        {
            backgroundImage.sprite = Resources.Load<Sprite>($"Sprites/UI/SelectBackground_Yellow");
        }
        foreach (GameObject star in stars)
        {
            star.SetActive(false);
        }
        for (int i = 0; i < creatureInfo.Star; i++)
        {
            stars[i].SetActive(true);
        }
    }
}
