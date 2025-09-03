using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Dungeon_UI : MonoBehaviour
{
    public GameObject Dungeon_Canvas;
    public Button Dungeon_FightButton;
    public Button Button_Back;
    void Awake()
    {
        Dungeon_FightButton.onClick.AddListener(OnButtonClick);
        Button_Back.onClick.AddListener(() => Dungeon_Canvas.SetActive(false));
    }

    void OnButtonClick()
    {
        Player.Instance.matchingCreatureInfo = EnemyCreatureController.Instance.combatEnemyCreatureInfo;
        SceneManager.LoadScene("DungeonScene");
        SoundManager.Instance.PlayBattleBGM();
        Player.Instance.battleType = BattleType.Dungeon;
    }
}
