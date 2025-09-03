using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CombatSystem : MonoBehaviour
{
    public Action<CombatEvent> effectEvent;
    //public Action<bool> onDamage;
    public static CombatSystem Instance;
    public Dictionary<Collider, IDamageAble> creatureDic = new Dictionary<Collider, IDamageAble>();
    private Queue<CombatEvent> combatEventQueue = new Queue<CombatEvent>();

    private void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        while (combatEventQueue.Count > 0)
        {
            CombatEvent combatEvent = combatEventQueue.Dequeue();

            if (combatEvent.UseEffect == true)
            {
                effectEvent.Invoke(combatEvent);
            }
            combatEvent.Receiver.TakeDamage(combatEvent.Damage, true);

        }
    }

    public void RegisterCreature(Collider collider, IDamageAble damageAble)
    {
        if (creatureDic.ContainsKey(collider) == false)
        {
            creatureDic.TryAdd(collider, damageAble);
        }
    }
    public IDamageAble GetCreatureOrNull(Collider collider)
    {
        Debug.Log(creatureDic[collider].GameObject.name);
        if (creatureDic.ContainsKey(collider))
        {
            return creatureDic[collider];
        }
        return null;
    }

    public void AddCombatEvent(CombatEvent combatEvent)
    {
        combatEventQueue.Enqueue(combatEvent);
    }
}