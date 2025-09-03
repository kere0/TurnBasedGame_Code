using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Ranking_Slot : MonoBehaviour
{
    public TextMeshProUGUI ranking_Text;
    public TextMeshProUGUI nickname_Text;
    public TextMeshProUGUI rating_Text;
    void Awake()
    {
        ranking_Text = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        nickname_Text = transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        rating_Text = transform.GetChild(2).GetComponent<TextMeshProUGUI>();
    }
    public void SetRanking(string ranking, string nickname, int rating)
    {
        ranking_Text.text = ranking;
        nickname_Text.text = nickname;
        rating_Text.text = rating.ToString();
    }
}
