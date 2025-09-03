using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Firebase.Firestore;
using UnityEngine;
using UnityEngine.UI;

public class Ranking_SlotController : MonoBehaviour
{
    List<Ranking_Slot> ranking_slot = new List<Ranking_Slot>();
    private List<GameObject> slotClearList = new List<GameObject>();
    private GameObject rankingPrefab;
    private List<string> nicknameList = new List<string>();
    private List<int> ratingList = new List<int>();
    public GameObject ranking_Canvas;
    public Button button_Back;
    async void Start()
    {
        rankingPrefab  = Resources.Load<GameObject>("RankingSlot");
        await RankingList_SlotSet();
        button_Back.onClick.AddListener(()=> ranking_Canvas.SetActive(false));
    }
    public async Task RankingList_SlotSet()
    {
        ranking_slot.Clear();
        foreach (var slot in slotClearList)
        {
            Destroy(slot);
        }
        slotClearList.Clear();
        int num = 0;
         List<string> allUserId = await Managers.SaveLoadFirebase.GetAllUserId();
         foreach (string userId in allUserId)
         {
             DocumentReference doRef = Managers.FirestoreManager.firestore.Collection("users").Document(userId).Collection("data").Document("userData");
             DocumentSnapshot snapshot = await doRef.GetSnapshotAsync();
             if (snapshot.Exists)
             {
                 if (snapshot.TryGetValue("rating", out object rating))
                 {
                     ratingList.Add(Convert.ToInt32(rating));
                 }
                 if (snapshot.TryGetValue("nickName", out object nickname))
                 {
                     nicknameList.Add(nickname.ToString());
                 }
             }
         }
         var userData = nicknameList.Zip(ratingList, (nickname, rating) => (nickname, rating)).ToList();
         var sorted = userData.OrderByDescending(x => x.rating).ToList();
         int rankingIndex = 0;
         foreach (var (nickname, rating) in sorted)
         {
             rankingIndex++;
             GameObject go = Instantiate(rankingPrefab, transform);
             slotClearList.Add(go);
             Ranking_Slot ranking_slot = go.GetComponent<Ranking_Slot>();
             ranking_slot.SetRanking(rankingIndex.ToString(), nickname, rating);
         }
    }
}
