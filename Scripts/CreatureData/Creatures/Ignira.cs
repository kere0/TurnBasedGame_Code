using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Ignira : BaseCreature
{
    public GameObject attackPos2;
    public Image bossHpBar;

    private void Start()
    {
        bossHpBar = GameObject.Find("BossHpBar").GetComponent<Image>();
    }
    public override void IdleState()
    {
        if (stateEnter == false)
        {
            stateEnter = true;
            attackType = AttackType.None;
            animator.Play("Idle");
        }
        transform.rotation = Quaternion.Slerp(transform.rotation, startRotation, Time.deltaTime * 10f);
        if (fireEnd.All(x => x))
        {
            for (int i = 0; i < fireEnd.Length; i++)
                fireEnd[i] = false;
            attackEnd.Invoke();
        }
    }
    protected override void Attack()
    {
        if (targetCreature == null || targetCreature.collider == null) return;
        IDamageAble targetEnemy = CombatSystem.Instance.GetCreatureOrNull(targetCreature.collider);
        CombatEvent combatEvent = new CombatEvent();

        if (attackType == AttackType.NormalAttack)
        {
            combatEvent.UseEffect = false;
            combatEvent.Damage = attack;
            combatEvent.Sender = this;
            combatEvent.Receiver = targetEnemy;
            CombatSystem.Instance.AddCombatEvent(combatEvent);
        }
        else if (attackType == AttackType.SkillAttack)
        {
            //Vector3 targetCreaturePos = targetEnemy.Collider.bounds.center;
            Vector3 dir = (targetEnemy.Collider.bounds.center - transform.position).normalized;

            SetCombatEvent(dir, targetEnemy.Collider.bounds.center, targetEnemy, attackPos.transform.position);
            SetCombatEvent(dir, targetEnemy.Collider.bounds.center, targetEnemy, attackPos2.transform.position);
            if (creatureTeam == CreatureTeam.Player)
            {
                combatCreatureSlot.StartCoolTime_Coroutine(skillCoolTime, this);
            }
            else if (creatureTeam == CreatureTeam.Enemy)
            {
                StartCoroutine(CoolTime());
            }
        }
    }
    void SetCombatEvent(Vector3 dir, Vector3 targetCreaturePos, IDamageAble targetEnemy, Vector3 attackPos)
    {
        GameObject fire = Managers.PoolManager.ObjPop(skillName, attackPos);
        fire.transform.rotation = Quaternion.LookRotation(dir, Vector3.up);
        BaseSkillEffect skillEffect = fire.GetComponent<BaseSkillEffect>();
        skillEffect.creature = this;
        skillEffect.target = targetCreature;
        skillEffect.targetPos = targetCreaturePos;
        skillEffect.skillDamage = skillAttack;
    }
    public override void TakeDamage(int damage, bool OnDamage)
    {
        ChangeState(CreatureState.Damage);
        currentHp -= damage;
        float ratio  = currentHp / (float)maxHp;
        ratio = Mathf.Clamp(ratio, 0f, 1f);
        bossHpBar.fillAmount = ratio;
        if(currentHp <= 0)
        {
            currentHp = 0;
            ChangeState(CreatureState.Die);
        }
    }
}
