using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase.Auth;
using Firebase.Firestore;
using UnityEngine;
using UnityEngine.Serialization;

public enum BattleType
{
    None,
    Dungeon,
    PVP
}
public class Player : MonoBehaviour
{
    public static Player Instance;
    // 내가 가진 모든 크리처들의 정보
    public List<SaveCreatureInfo> totalCreatureInfo = new List<SaveCreatureInfo>(); // 나중에 저장할때 이걸로 저장, 시작할때도 여기에 값 받아옴
    public List<SaveCreatureInfo> combatCreatureInfo = new List<SaveCreatureInfo>(); // 전투에 사용할 크리처 3마리
    public List<SaveCreatureInfo> unCombatCreatureInfo = new List<SaveCreatureInfo>(); // 전투에 사용할 크리처 3마리
    public List<SaveCreatureInfo> matchingCreatureInfo = new List<SaveCreatureInfo>();
    [HideInInspector] public int level = 1;
    [HideInInspector] public float exp;
    [HideInInspector] public int money = 1000;
    [HideInInspector] public int runeCount;
    [HideInInspector] public string nickName;
    [HideInInspector] public int rating = 1000;
    public BattleType battleType = BattleType.None;
    public string matchedPlayerId;
    public int matchedPlayerRating;
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private async void Start()
    {
        DocumentSnapshot snapshot = await GetTotalPlayerDeckDocRef("combatCreatureInfo").GetSnapshotAsync();
        if (snapshot.Exists == false)
        {
            await MyCreatureSet();
            CreatureSave_Firebase();
            runeCount = 30;
            Managers.SaveLoadFirebase.PlayerDataSave(FirebaseAuth.DefaultInstance.CurrentUser.UserId.ToString());
        }
        else
        {
            await Managers.SaveLoadFirebase.CreatureLoad_FireBase(FirebaseAuth.DefaultInstance.CurrentUser.UserId.ToString(),
             "combatCreatureInfo", (loadCombatCreature) =>
            {
                combatCreatureInfo = loadCombatCreature;
            });
            await Managers.SaveLoadFirebase.CreatureLoad_FireBase(FirebaseAuth.DefaultInstance.CurrentUser.UserId.ToString(),
             "unCombatCreatureInfo", (loadCombatCreature) =>
            {
                unCombatCreatureInfo = loadCombatCreature;
            });
        }
        SetTotalCreature();
        await Managers.SaveLoadFirebase.PlayerDataLoad(FirebaseAuth.DefaultInstance.CurrentUser.UserId.ToString());
        UserInfo_UI.Instance.SetUserInfo();
    }

    public void SetTotalCreature()
    {
        totalCreatureInfo.Clear();
        foreach (SaveCreatureInfo combatCreature in combatCreatureInfo)
        {
            totalCreatureInfo.Add(combatCreature);
        }
        foreach (SaveCreatureInfo unCombatCreature in unCombatCreatureInfo)
        {
            totalCreatureInfo.Add(unCombatCreature);            
        }
    }
    private DocumentReference GetTotalPlayerDeckDocRef(string documentName)
    {
        return Managers.FirestoreManager.firestore
            .Collection("users")
            .Document(FirebaseAuth.DefaultInstance.CurrentUser.UserId.ToString())
            .Collection("creatures")
            .Document(documentName);
    }
    public async Task MyCreatureSet()
    {
        // Firebase를 통해 creatureInfo를 세팅해준 후
        // 크리처 Select창에서 combatCreatureInfo에 값 할당 
        SaveCreatureInfo saveCreatureInfo = new SaveCreatureInfo()
        {
            ID = "Blawlf",
            Star = 1,
            Rarity = Rarity.Legend,
            isUsing = true,
        };
        combatCreatureInfo.Add(saveCreatureInfo);
        
        SaveCreatureInfo saveCreatureInfo1 = new SaveCreatureInfo()
        {
            ID = "Drafi",
            Star = 1,
            Rarity = Rarity.Normal,
            isUsing = true,
        };
        combatCreatureInfo.Add(saveCreatureInfo1);
        
        SaveCreatureInfo saveCreatureInfo2 = new SaveCreatureInfo()
        {
            ID = "Blawlf",
            Star = 1,
            Rarity = Rarity.Epic,
            isUsing = true,
        };
        combatCreatureInfo.Add(saveCreatureInfo2);
        
        SaveCreatureInfo saveCreatureInfo3 = new SaveCreatureInfo()
        {
            ID = "Smofu",
            Star = 1,
            Rarity = Rarity.Legend,
            isUsing = false,
        };
        unCombatCreatureInfo.Add(saveCreatureInfo3);
    }
    public async Task CreatureSave_Firebase()
    {
        Managers.SaveLoadFirebase.CreatureSave_FireBase(FirebaseAuth.DefaultInstance.CurrentUser.UserId.ToString(), combatCreatureInfo, "combatCreatureInfo");
        Managers.SaveLoadFirebase.CreatureSave_FireBase(FirebaseAuth.DefaultInstance.CurrentUser.UserId.ToString(), unCombatCreatureInfo, "unCombatCreatureInfo");
        Managers.SaveLoadFirebase.PlayerDataSave(FirebaseAuth.DefaultInstance.CurrentUser.UserId.ToString());
    }
}
