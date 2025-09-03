using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Firebase.Auth;
using Firebase.Firestore;
using UnityEngine;
using Random = UnityEngine.Random;

public class SaveLoad_Firebase 
{
    public async Task<List<string>> GetAllUserId()
    {
        CollectionReference usersRef = Managers.FirestoreManager.firestore.Collection("users");
        QuerySnapshot allUsersSnap = await usersRef.GetSnapshotAsync();
        
        List<string> allUserIds = allUsersSnap.Documents.Select(doc => doc.Id).ToList();
        return allUserIds;
    }
    public void CreatureSave_FireBase(string UserId, List<SaveCreatureInfo> creatureList, string documentName) // 처음에 모든 크리처 정보 저장
    {
        List<SaveCreatureInfo> creatures = new List<SaveCreatureInfo>();
        foreach (SaveCreatureInfo creature in creatureList)
        {
            creatures.Add(creature);
        }
        MyCreatureDataList wrapper = new MyCreatureDataList()
        {
            CreatureData = creatures
        };
        Managers.FirestoreManager.firestore.Collection("users").Document(UserId).Collection("creatures").Document(documentName).SetAsync(wrapper);
    }
    public async Task CreatureLoad_FireBase(string userId, string documnetName, Action<List<SaveCreatureInfo>> onDeckLoaded) // 모든 카드데이터 현제 스텟데이터 세팅해서 가져옴
    {
        DocumentReference doRef = Managers.FirestoreManager.firestore.Collection("users").Document(userId).Collection("creatures").Document(documnetName);
        
        DocumentSnapshot snapshot = await doRef.GetSnapshotAsync();
        if (snapshot.Exists)
        {
            MyCreatureDataList data = snapshot.ConvertTo<MyCreatureDataList>();
            List<SaveCreatureInfo> loadData = data.CreatureData;
            onDeckLoaded?.Invoke(loadData);
        }
    }
    public void PlayerDataSave(string UserId)
    {
        Dictionary<string, object> data = new Dictionary<string, object>()
        {
            { "money", Player.Instance.money },
            { "rune", Player.Instance.runeCount },
            {"level", Player.Instance.level },
            { "exp", Player.Instance.exp },
            { "nickName", Player.Instance.nickName},
            { "rating", Player.Instance.rating}
        };
        Managers.FirestoreManager.firestore.Collection("users").Document(UserId).Collection("data").Document("userData").SetAsync(data, SetOptions.MergeAll);
    }
    public async Task PlayerDataLoad(string UserId)
    {
        DocumentReference doRef = Managers.FirestoreManager.firestore.Collection("users").Document(UserId).Collection("data").Document("userData");

        DocumentSnapshot snapshot = await doRef.GetSnapshotAsync();
        if (snapshot.Exists)
        {
            if (snapshot.TryGetValue("money", out object money))
            {
                Player.Instance.money = Convert.ToInt32(money);
            }
            if (snapshot.TryGetValue("rune", out object runeCount))
            {
                Player.Instance.runeCount = Convert.ToInt32(runeCount);
            }
            if (snapshot.TryGetValue("level", out object level))
            {
                Player.Instance.level = Convert.ToInt32(level);
            }
            if (snapshot.TryGetValue("exp", out object exp))
            {
                Player.Instance.exp = Convert.ToSingle(exp);
            }
            if (snapshot.TryGetValue("nickName", out object nickName))
            {
                Player.Instance.nickName = nickName.ToString();
            }
            if (snapshot.TryGetValue("rating", out object rating))
            {
                Player.Instance.rating = Convert.ToInt32(rating);
            }
        }
    }
}
