using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
    public static EffectManager Instance;
    public List<GameObject> effectPrefabs;

    private void Awake()
    {
        Instance = this;
        CombatSystem.Instance.effectEvent -= PlayEffect;
        CombatSystem.Instance.effectEvent += PlayEffect;
    }
    public void Start()
    { 
        Managers.PoolManager.pools.Clear();
        effectPrefabs.Add(Resources.Load<GameObject>("Skills/BlawlfSkill"));
        effectPrefabs.Add(Resources.Load<GameObject>("Skills/DrafiSkill"));
        effectPrefabs.Add(Resources.Load<GameObject>("Skills/SmofuSkill"));
        effectPrefabs.Add(Resources.Load<GameObject>("Skills/BossSkill"));
        effectPrefabs.Add(Resources.Load<GameObject>("Skills/BossShield"));
        for (int i = 0; i < effectPrefabs.Count; i++)
        {
            Managers.PoolManager.ObjInit(effectPrefabs[i], 3);
        }
    }
    public void PlayEffect(CombatEvent ev)
    {
        GameObject go = Managers.PoolManager.ObjPop(ev.EffectName,ev.EffectPosition);
        //go.transform.parent = ev.Receiver.GameObject.transform;
        go.transform.rotation =Quaternion.LookRotation(ev.EffectRotation, Vector3.up);
        go.GetComponent<BaseSkillEffect>().creature = ev.Sender.GameObject.GetComponent<BaseCreature>();
    }
}