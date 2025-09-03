using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Firebase.Auth;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;
public class GameClear_UI : MonoBehaviour
{
    public GameObject[] stars = new GameObject[3]; 
    public GameObject[] starParticles = new GameObject[3];
    public Image starBackground;
    public GameObject[] reward_items = new GameObject[3];
    private Image[] rewardItem_image = new Image[3];
    private TextMeshProUGUI[] rewardItem_text = new TextMeshProUGUI[3];
    public TextMeshProUGUI result_text;
    private Button claimButton;
    private Slider expBar;
    public TextMeshProUGUI level_Text;
    public TextMeshProUGUI exp_Text;
    string[] creatureIDs = {"Blawlf", "Drafi", "Smofu"};
    private float duration = 2f;
    private float elapsedTime = 0f;
    private float getExp = 0f;
    private float startValue;
    private float targetValue;
    public GameObject gameClear_Canvas;
    private bool onClaimButton = false;
    void Start()
    {
        for (int i = 0; i < rewardItem_image.Length; i++)
        {
            rewardItem_image[i] = reward_items[i].transform.GetChild(0).GetComponent<Image>();
        }
        for (int i = 0; i < rewardItem_text.Length; i++)
        {
            rewardItem_text[i] = reward_items[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        }
        claimButton = transform.GetChild(5).GetComponent<Button>();
        expBar = transform.GetChild(3).GetComponent<Slider>();
        claimButton.onClick.AddListener(OnClaimButton);
        level_Text.text = Player.Instance.level.ToString();
        exp_Text.text = $"{expBar.value}/{expBar.maxValue}";
        GetReward();
        SetStar();
    }
    private void Update()
    {
        if (elapsedTime < duration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            float t = elapsedTime / duration;
            float lerpedValue = Mathf.Lerp(startValue, targetValue, t);
            exp_Text.text = $"{Mathf.RoundToInt(expBar.value*100)}/{Mathf.RoundToInt(expBar.maxValue*100)}";
            if (lerpedValue >= 1f)
            { 
                float overflow = targetValue - 1;
                expBar.value = 1f;
                Player.Instance.level += 1;
                level_Text.text = Player.Instance.level.ToString();
                startValue = 0f;
                targetValue = overflow;
                elapsedTime = 0f;
            }
            else
            {
                expBar.value = lerpedValue;
            }
            Player.Instance.exp = expBar.value;
            Managers.SaveLoadFirebase.PlayerDataSave(FirebaseAuth.DefaultInstance.CurrentUser.UserId.ToString());
        }
        else
        {
            onClaimButton = true;
        }
    }
    void OnClaimButton()
    {
        if (onClaimButton == true)
        {
            gameClear_Canvas.SetActive(false);
            Time.timeScale = 1;
            SceneManager.LoadScene("LobbyScene");
            SoundManager.Instance.PlayNormalBGM();
            onClaimButton = false;
        }
    }
    void SetStar()
    {
        for (int i = 0; i < stars.Length; i++)
        {
            stars[i].GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/StarClose");
            starParticles[i].SetActive(false);
        }
        int aliveCreature = GameManager.Instance.myFieldCreature.Count(c => c.IsDie == false);
        for (int i = 0; i < aliveCreature; i++)
        {
            stars[i].GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Star");
            starParticles[i].SetActive(true);
        }

        if (aliveCreature == 0)
        {
            result_text.text = "STAGE FAILED!";
            Color color = starBackground.color;
            color.a = 0f;
            starBackground.color = color;
        }
        else
        {
            result_text.text = "STAGE CLEAR!";
        }
    }
    void GetReward()
    {
        int aliveCreature = GameManager.Instance.myFieldCreature.Count(c => c.IsDie == false);
        startValue = Player.Instance.exp;
        if ((aliveCreature == 0))
        {
            getExp = 0;
            reward_items[0].gameObject.SetActive(false);
            reward_items[1].gameObject.SetActive(false);
        }
        else
        {
            getExp = 30;
            int random_Gold = Random.Range(100, 1301);
            Player.Instance.money += random_Gold;
            UserInfo_UI.Instance.SetUserInfo();
            rewardItem_text[0].text = random_Gold.ToString();
        
            int random_Rune = Random.Range(1, 6);
            Player.Instance.runeCount += random_Rune;
            Managers.SaveLoadFirebase.PlayerDataSave(FirebaseAuth.DefaultInstance.CurrentUser.UserId.ToString());
            rewardItem_text[1].text = random_Rune.ToString();

            int randomCreature = Random.Range(0, 3);
            if(randomCreature == 1)
            {
                reward_items[2].SetActive(true);
                Rarity[] value = (Rarity[])Enum.GetValues(typeof(Rarity));
                SaveCreatureInfo saveCreatureInfo = new SaveCreatureInfo()
                {
                    ID = creatureIDs[Random.Range(0, creatureIDs.Length)],
                    Star = 1,
                    Rarity = value[Random.Range(0, value.Length)],
                    isUsing = false,
                };
                rewardItem_image[2].sprite = Resources.Load<Sprite>($"Sprites/Creatures/{saveCreatureInfo.ID}_{saveCreatureInfo.Rarity}");
                Player.Instance.unCombatCreatureInfo.Add(saveCreatureInfo);
                Player.Instance.SetTotalCreature();
                Player.Instance.CreatureSave_Firebase();
            }
        }
        targetValue = Player.Instance.exp + (getExp / 100f);
    }
}
