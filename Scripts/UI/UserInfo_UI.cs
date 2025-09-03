using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase.Auth;
using Firebase.Firestore;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UserInfo_UI : MonoBehaviour
{
    public static UserInfo_UI Instance;
    public TextMeshProUGUI MainCanvasMoney_Text;
    public TextMeshProUGUI UpgradeCanvasMoney_Text;
    public TextMeshProUGUI DungeonMoney_Text;
    public TextMeshProUGUI Level_Text;
    public Slider Exp_Slider;
    public TextMeshProUGUI Exp_Text;
    public GameObject NickName_Popup;
    public TextMeshProUGUI NickName_Text;
    public TMP_InputField NickName_InputField;
    public Button OkButton;
    private void Awake()
    {
        Instance = this;
        OkButton.onClick.AddListener(SetUserNickName);
    }
    private void Start()
    {
        SetNickName();
        SetUserInfo();
    }
    async Task SetNickName()
    {
        DocumentSnapshot snapshot = await Managers.FirestoreManager.firestore.Collection("users")
            .Document(FirebaseAuth.DefaultInstance.CurrentUser.UserId).Collection("data").Document("userData").GetSnapshotAsync();
        if (snapshot.Exists == false)
        {
            NickName_Popup.SetActive(true);
        }
    }
    private void SetUserNickName()
    {
        Player.Instance.nickName = NickName_InputField.text;
        Managers.SaveLoadFirebase.PlayerDataSave(FirebaseAuth.DefaultInstance.CurrentUser.UserId.ToString());
        NickName_Text.text = Player.Instance.nickName;
        NickName_Popup.SetActive(false);
    }
    public void SetUserInfo()
    {
        MainCanvasMoney_Text.text = Player.Instance.money.ToString("N0");
        UpgradeCanvasMoney_Text.text = Player.Instance.money.ToString("N0");
        DungeonMoney_Text.text = Player.Instance.money.ToString("N0");
        Level_Text.text= Player.Instance.level.ToString();
        Exp_Text.text = $"{Mathf.RoundToInt(Player.Instance.exp*100)}/100";
        Exp_Slider.value = Player.Instance.exp;
        NickName_Text.text = Player.Instance.nickName;
    }
}
