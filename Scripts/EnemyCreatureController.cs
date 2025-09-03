using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCreatureController : MonoBehaviour
{
    public static EnemyCreatureController Instance;
    public List<SaveCreatureInfo> combatEnemyCreatureInfo = new List<SaveCreatureInfo>(); // 전투에 사용할 크리처 3마리
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
    void Start()
    {
        EnemyCreatureSet();
    }
    void EnemyCreatureSet()
    {
        SaveCreatureInfo saveCreatureInfo = new SaveCreatureInfo();
        saveCreatureInfo.ID = "Ignira";
        saveCreatureInfo.Star = 3;
        saveCreatureInfo.Rarity = Rarity.Legend;
        combatEnemyCreatureInfo.Add(saveCreatureInfo);
    }
}