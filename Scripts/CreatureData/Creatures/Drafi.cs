using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drafi : BaseCreature
{
    public override void IdleState()
    {
        if (stateEnter == false)
        {
            stateEnter = true;
            attackType = AttackType.None;
            animator.Play("Idle");
        }
        transform.rotation = Quaternion.Slerp(transform.rotation, startRotation, Time.deltaTime * 10f);
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
            GameObject fire = Managers.PoolManager.ObjPop(skillName, attackPos.transform.position);
            //Vector3 targetCreaturePos = targetCreature.transform.position + new Vector3(0f,0.5f,0f);
            Vector3 dir = (targetEnemy.Collider.bounds.center - transform.position).normalized;
            fire.transform.rotation = Quaternion.LookRotation(dir, Vector3.up);
            BaseSkillEffect skillEffect = fire.GetComponent<BaseSkillEffect>();
            skillEffect.creature = this;
            skillEffect.target = targetCreature;
            skillEffect.targetPos = targetEnemy.Collider.bounds.center;
            skillEffect.skillDamage = skillAttack;
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
}
