using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Firebase.Auth;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    private bool isBattleStarted = false;
    // 크리처가 생성될 위치
    public Transform[] myBattleFieldPosition = new Transform[3];
    public Transform[] enemyBattleFieldPosition = new Transform[3];
    // 맵에 소환된 크리처
    public List<BaseCreature> myFieldCreature = new List<BaseCreature>();
    public List<BaseCreature> enemyFieldCreature = new List<BaseCreature>();
    public int myCreatureCount = 0;
    public int enemyCreatureCount = 0;
    private BaseCreature playerAttacker;
    private BaseCreature enemyAttacker;
    private BaseCreature curentAttacker;
    private CreatureTeam currentAttackerTeam = CreatureTeam.None;
    private bool combatStart = false;
    private float startDelay = 3f;
    public GameObject gameClear_Canvas;
    public GameObject PVPResult_Canvas;
    private bool onGameClearUI = false;
    // private List<SaveCreatureInfo> matchingEnemy = new List<SaveCreatureInfo>();
    public bool matchingEnemyInit = false;
    private float GameClearTime;
    public GameObject bossHpBar_UI;
    void Awake()
    {
        Instance = this;
    }
    private void Update()
    {
        if (Player.Instance != null)
        {
            if (isBattleStarted == false)
            {
                isBattleStarted = true;
                MyCreatureInit();
                EnemyCreatureInit(Player.Instance.matchingCreatureInfo);
                //EnemyCreatureInit(EnemyCreatureController.Instance.combatEnemyCreatureInfo);
                //MatchingEnemy();
            }
        }
        if (isBattleStarted == true && combatStart == false)
        {
            startDelay -= Time.deltaTime;
            if (startDelay <= 0)
            {
                combatStart = true;
                SetAttacker();
            }
        }
        GameClear();
        SetGameSpeed();
    }
    private void SetGameSpeed()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Time.timeScale = 1;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Time.timeScale = 2;
        }
    }
    public void SetAttacker()
    {
        playerAttacker = myFieldCreature.Where(c=> c.die == false && c.isTurnUsed == false).OrderByDescending(c => c.attackSpeed).FirstOrDefault();
        enemyAttacker = enemyFieldCreature.Where(c=> c.die == false && c.isTurnUsed == false).OrderByDescending(c => c.attackSpeed).FirstOrDefault();
        if (playerAttacker != null || enemyAttacker != null)
        {
            // 첫턴
            if (currentAttackerTeam == CreatureTeam.None)
            {
                curentAttacker = playerAttacker.attackSpeed >= enemyAttacker.attackSpeed ? playerAttacker : enemyAttacker;
                curentAttacker.AttackTurn();
                currentAttackerTeam = curentAttacker.creatureTeam == CreatureTeam.Player ? CreatureTeam.Player : CreatureTeam.Enemy;
            }
            else if (currentAttackerTeam != CreatureTeam.None)
            {
                if (playerAttacker != null && enemyAttacker != null)
                {
                    curentAttacker = curentAttacker.creatureTeam == CreatureTeam.Player ? enemyAttacker : playerAttacker;
                }
                else
                {
                    curentAttacker = playerAttacker == null ? enemyAttacker : playerAttacker;
                }
                curentAttacker.AttackTurn();
                currentAttackerTeam = curentAttacker.creatureTeam == CreatureTeam.Player ? CreatureTeam.Player : CreatureTeam.Enemy;
            }
        }
        else
        {
            foreach (var myCreature in myFieldCreature)
            {
                myCreature.isTurnUsed = false;
            }

            foreach (var enemyCreature in enemyFieldCreature)
            {
                enemyCreature.isTurnUsed = false;
            }
            SetAttacker();
        }
    }
    void MyCreatureInit()
    {
        int num = 0;
        foreach (SaveCreatureInfo combatCreature in Player.Instance.combatCreatureInfo)
        {
            Debug.Log(combatCreature.Rarity);
            GameObject prefab = Resources.Load<GameObject>($"Creature_Prefabs/{combatCreature.ID}_{combatCreature.Rarity}");
            Quaternion lookRotation = Quaternion.LookRotation(Vector3.left);
           
            GameObject go = Instantiate(prefab, myBattleFieldPosition[myCreatureCount].position, lookRotation);
            Managers.HPBarManager.Register(go.transform);
            BaseCreature baseCreature = go.GetComponent<BaseCreature>();
            baseCreature.SetInfo(combatCreature.ID, combatCreature.Rarity, combatCreature.Star);
            myFieldCreature.Add(baseCreature);
            baseCreature.creatureTeam = CreatureTeam.Player;
            baseCreature.die = false;
            baseCreature.combatCreatureSlot = CombatCreature_SlotController.Instance.combatCreatureSlot[num];
            num++;
            go.tag = "Player";
            
            Debug.Log(myCreatureCount);
            myCreatureCount++;
        }
    }
    async Task EnemyCreatureInit(List<SaveCreatureInfo> enemy)
    {
        foreach (SaveCreatureInfo combatCreature in enemy)
        {
            Debug.Log(combatCreature.Rarity);
            GameObject prefab = Resources.Load<GameObject>($"Creature_Prefabs/{combatCreature.ID}_{combatCreature.Rarity}");
            Quaternion lookRotation = Quaternion.LookRotation(Vector3.right);
            GameObject go = Instantiate(prefab, enemyBattleFieldPosition[enemyCreatureCount].position, lookRotation);
            if(Player.Instance.battleType == BattleType.Dungeon)
            {
                bossHpBar_UI.SetActive(true);
            }
            else if(Player.Instance.battleType == BattleType.PVP)
            {
                Managers.HPBarManager.Register(go.transform);
            }
            BaseCreature baseCreature = go.GetComponent<BaseCreature>();
            baseCreature.SetInfo(combatCreature.ID, combatCreature.Rarity, combatCreature.Star);
            enemyFieldCreature.Add(baseCreature);
            baseCreature.creatureTeam = CreatureTeam.Enemy;
            baseCreature.die = false;
            go.tag = "Enemy";
            Debug.Log(enemyCreatureCount);
            enemyCreatureCount++;
        }
        matchingEnemyInit = true;
    }

    void GameClear()
    {
        if (onGameClearUI == false && matchingEnemyInit == true)
        {
            int aliveEnemyCreature = enemyFieldCreature.Count(c => c.die == false);
            int alivePlayerCreature =  myFieldCreature.Count(c=>c.die == false);
            if (alivePlayerCreature == 0 || aliveEnemyCreature == 0)
            {
                if (GameClearTime <= 1f)
                {
                    GameClearTime += Time.deltaTime;
                }
                else
                {
                    onGameClearUI = true;
                    Time.timeScale = 0;
                    if (Player.Instance.battleType == BattleType.Dungeon)
                    {
                        gameClear_Canvas.SetActive(true);
                    }
                    else if (Player.Instance.battleType == BattleType.PVP)
                    {
                        PVPResult_Canvas.SetActive(true);
                    }
                    matchingEnemyInit = false;
                }
                
            }
        }
    }
}
