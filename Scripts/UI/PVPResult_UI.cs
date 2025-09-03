using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Firebase.Auth;
using Firebase.Firestore;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;
public class PVPResult_UI : MonoBehaviour
{
    public Image starBackground;
    public GameObject[] stars = new GameObject[3]; 
    public GameObject[] starParticles = new GameObject[3];
    public TextMeshProUGUI result_Text;
    private Button claimButton;
    private float duration = 2f;
    private float elapsedTime = 0f;
    private int getRating;
    private int startValue;
    private int targetValue;
    public GameObject pvpResult_Canvas;
    private bool onClaimButton = false;
    private bool isVictory = false;
    public TextMeshProUGUI rating_Text;
    void Start()
    {
        claimButton = transform.GetChild(3).GetComponent<Button>();
        claimButton.onClick.AddListener(OnClaimButton);
        SetStar();
        GetReward();
        startValue = Player.Instance.rating;
        targetValue = Player.Instance.rating + getRating;
        rating_Text.text = Player.Instance.rating.ToString();
    }
    private void Update()
    {
        if (elapsedTime < duration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            float t = elapsedTime / duration;
            int rating = Mathf.RoundToInt(Mathf.Lerp(startValue, targetValue, t));
            rating_Text.text = rating.ToString();
            Player.Instance.rating = rating;
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
            pvpResult_Canvas.SetActive(false);
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
            Color color = starBackground.color;
            color.a = 0;
            starBackground.color = color;
            result_Text.text = "Lose";
            isVictory = false;
        }
        int aliveEnemyCreature = GameManager.Instance.enemyFieldCreature.Count(c => c.IsDie == false);
        if (aliveEnemyCreature == 0)
        {
            result_Text.text = "Victory";
            isVictory = true;
        }
    }
    void GetReward()
    {
        if (isVictory == true)
        {
            getRating = 30;
            Dictionary<string, object> data = new Dictionary<string, object>()
            {
                { "rating", Player.Instance.matchedPlayerRating - getRating}
            };
            Managers.FirestoreManager.firestore.Collection("users").Document(Player.Instance.matchedPlayerId)
                .Collection("data").Document("userData").SetAsync(data, SetOptions.MergeAll);
        }
        else
        {
            getRating = -30;
            int rating;
            if (Player.Instance.matchedPlayerRating + getRating <= 0)
            {
                rating = 0;
            }
            else
            {
                rating = Player.Instance.matchedPlayerRating + getRating;
            }
            Dictionary<string, object> data = new Dictionary<string, object>()
            {
                { "rating", rating}
            };
            Managers.FirestoreManager.firestore.Collection("users").Document(Player.Instance.matchedPlayerId)
                .Collection("data").Document("userData").SetAsync(data, SetOptions.MergeAll);
        }
        UserInfo_UI.Instance.SetUserInfo();
        Managers.SaveLoadFirebase.PlayerDataSave(FirebaseAuth.DefaultInstance.CurrentUser.UserId.ToString());
        Player.Instance.SetTotalCreature();
        Player.Instance.CreatureSave_Firebase();
        
    }
}
