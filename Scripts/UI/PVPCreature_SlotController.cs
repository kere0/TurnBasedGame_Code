using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Firebase.Auth;
using Firebase.Firestore;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class PVPCreature_SlotController : MonoBehaviour
{
    public static PVPCreature_SlotController Instance;
    public GameObject PVP_Canvas;
    public Button matchingButton;
    public Button button_Back;
    public PVPCreature_Slot[] playerSlot = new PVPCreature_Slot[3];
    public PVPCreature_Slot[] enemySlot = new PVPCreature_Slot[3];
    private List<SaveCreatureInfo> matchingEnemy = new List<SaveCreatureInfo>();
    public TextMeshProUGUI playerId;
    public TextMeshProUGUI playerRating;

    public TextMeshProUGUI enemyId;
    public TextMeshProUGUI enemyRating;
    public GameObject enemyPopup;
    private RectTransform enemyPopupRectTransform;
    private Vector3 startPopupPos;
    float duration = 2;
    private float elapsedTime;
    private bool matchingButtonClick = false;
    void Awake()
    {
        Instance = this;
        matchingButton.onClick.AddListener(OnMatchingButton);
        button_Back.onClick.AddListener(OnBackButton);
        enemyPopup.TryGetComponent(out enemyPopupRectTransform);
    }
    private void Start()
    {
        SetCreatureInfo();
        playerId.text = Player.Instance.nickName;
        playerRating.text = Player.Instance.rating.ToString();
        startPopupPos = enemyPopupRectTransform.anchoredPosition;
    }
    public void SetCreatureInfo()
    {
        for (int i = 0; i < 3; i++)
        {
            playerSlot[i].creatureInfo = Player.Instance.combatCreatureInfo[i];
            playerSlot[i].SetSlot();
        }
    }
    void OnBackButton()
    {
        if (matchingButtonClick == false)
        {
            PVP_Canvas.SetActive(false);
        }
    }
    async void OnMatchingButton()
    {
        if (matchingButtonClick == false)
        {
            matchingButtonClick = true;
            await MatchingEnemy();
            await Task.Delay(3000);
            Player.Instance.battleType = BattleType.PVP;
            SceneManager.LoadScene("DungeonScene");
            SoundManager.Instance.PlayBattleBGM();
        }
    }
    async Task MatchingEnemy()
    {
        List<string> tempUserIdList = await Managers.SaveLoadFirebase.GetAllUserId();
        List<string> enemyUserIdList = tempUserIdList.Where(c=> c != FirebaseAuth.DefaultInstance.CurrentUser.UserId).ToList();
        int randomIndex = Random.Range(0, enemyUserIdList.Count);
        string enemyUserId = enemyUserIdList[randomIndex];
        Player.Instance.matchedPlayerId = enemyUserId;
        await Managers.SaveLoadFirebase.CreatureLoad_FireBase(enemyUserId, "combatCreatureInfo", (loadCombatCreature) =>
            {
                matchingEnemy = loadCombatCreature;
            });
        Player.Instance.matchingCreatureInfo = matchingEnemy;
        DocumentReference doRef = Managers.FirestoreManager.firestore.Collection("users").Document(enemyUserId).Collection("data").Document("userData");
        DocumentSnapshot snapshot = await doRef.GetSnapshotAsync();
        if (snapshot.Exists)
        {
            if (snapshot.TryGetValue("nickName", out object nickName))
            {
                enemyId.text = nickName.ToString();
            }
            if (snapshot.TryGetValue("rating", out object rating))
            {
                enemyRating.text = Convert.ToInt32(rating).ToString();
                Player.Instance.matchedPlayerRating = Convert.ToInt32(rating);
            }
        }
        SetCreatureSlot();
        await SetEnemyPopup();
    }
    public void SetCreatureSlot()
    {
        for (int i = 0; i < 3; i++)
        {
            enemySlot[i].creatureInfo = Player.Instance.matchingCreatureInfo[i];
            enemySlot[i].SetSlot();
        }
    }
    async Task SetEnemyPopup()
    {
        float duration = 2f;
        float elapsed = 0f;                               
        Vector3 startPos = startPopupPos;
        Vector3 endPos   = new Vector3(-365, -59, 0);

        while (elapsed < duration)
        {
            await Task.Yield();                            
            elapsed += Time.deltaTime;                     
            float t = Mathf.Clamp01(elapsed / duration);   
            enemyPopupRectTransform.anchoredPosition  = Vector3.Lerp(startPos, endPos, t);
        }
        enemyPopupRectTransform.anchoredPosition  = endPos;
    }
}
