using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

public enum CreatureState
{
    Idle,
    Move,
    Attack,
    Damage,
    Die
}

public enum CreatureTeam
{
    None,
    Player,
    Enemy
}

public enum AttackType
{
    None,
    NormalAttack,
    SkillAttack
}
public class BaseCreature : MonoBehaviour, IDamageAble
{
    // 이걸가진 게임오브젝트의 이름을 키값으로 정보받아오도록
    // 기본 세팅값은 star는 1이고 강화 성공시 csv에서 데이터 다시받아오도록
    // 시작할때는 Firebase를 통해서 세팅값 설정
    public GameObject GameObject => gameObject;
    public Collider Collider => collider;
    public float Hp => currentHp;
    public bool IsDie => die;
    //////////////////////////////////////////////////////////////
    public CreatureTeam creatureTeam;
    public GameObject attackPos;
    public Animator animator;
    private AnimatorStateInfo animatorState;
    private Rigidbody rigidbody;
    public Collider collider;
    private string id;
    private string star;
    private Rarity rarity;
    public int maxHp;
    public int currentHp;
    public int attack;
    public int skillAttack; // 해야됨
    public int attackSpeed;
    public string skillName;
    //public int skillDamage;
    public float skillCoolTime;
    private bool hasSkill;
    private bool passive;
    public bool die = false;
    public BaseCreature targetCreature;
    private Vector3 targetPos;
    private CreatureState currentState = CreatureState.Idle;
    public bool stateEnter = false;
    Vector3 dir = Vector3.zero;
    private bool isAttackRange = false;
    private Vector3 startPos;
    private bool isAttacking = false;
    private HPBar hpBar;
    public Action attackEnd;
    public bool isTurnUsed = false;
    public AttackType attackType = AttackType.None;
    public bool skillCoolTimeCheck = false;
    public Quaternion startRotation;
    public bool[] fireEnd = new bool[2] {false, false};
    public CombatCreature_Slot combatCreatureSlot;
    private float coolTime;
    Vector3 destination;
    private void Awake()
    {
        TryGetComponent(out animator);
        TryGetComponent(out rigidbody);
        TryGetComponent(out collider);
        startPos = transform.position;
        attackEnd -= GameManager.Instance.SetAttacker;
        attackEnd += GameManager.Instance.SetAttacker;
    }
    public void SetInfo(string id, Rarity rarity, int star)
    {
        // 이건 전투씬 들어갈때 값 부여
        CreatureData creatureData = Managers.CSVLoader.Get<CreatureData>(id, rarity, star);
        maxHp = creatureData.hp;
        currentHp = maxHp;
        attack = creatureData.attack;
        attackSpeed = creatureData.attackSpeed;
        skillName = creatureData.skillName;
        skillAttack = creatureData.skillAttack;
        skillCoolTime = creatureData.skillCoolTime;
        hasSkill = creatureData.hasSkill;
        hpBar = Managers.HPBarManager.GetHpBar(transform);
        CombatSystem.Instance.RegisterCreature(collider, this);
        startRotation = transform.rotation;
    }
    private void Update()
    {
        HandleState();
        animatorState = animator.GetCurrentAnimatorStateInfo(0);
    }
    public void HandleState()
    {
        switch(currentState)
        {
            case CreatureState.Idle:
                IdleState();
                break;
            case CreatureState.Move:
                MoveState();
                break;
            case CreatureState.Attack:
                AttackState();
                break;
            case CreatureState.Damage:
                DamageState();
                break;
            case CreatureState.Die:
                DieState();
                break;
        }
    }
    public void AttackTurn()
    {
        if (currentState == CreatureState.Idle)
        {
            isAttacking = true;
            isTurnUsed = true;
            ChangeState(CreatureState.Move);
        }
    }
    public virtual void IdleState()
    {
        if (stateEnter == false)
        {
            stateEnter = true;
            attackType = AttackType.None;
            animator.Play("Idle");
        }
        transform.rotation = Quaternion.Slerp(transform.rotation, startRotation, Time.deltaTime * 10f);
    }
    private void MoveState()
    {
        if (stateEnter == false)
        {
            stateEnter = true;
            animator.Play("Move");
            if (isAttacking == true)
            {
                if (skillCoolTimeCheck == false && hasSkill == true)
                {
                    attackType = AttackType.SkillAttack;
                }
                else
                {
                    attackType = AttackType.NormalAttack;
                }
                if (attackType == AttackType.NormalAttack)
                {
                    // 플레이어일때
                    if (creatureTeam == CreatureTeam.Player)
                    {
                        DetectTarget(GameManager.Instance.enemyFieldCreature);
                    }
                    // 적일때
                    else if (creatureTeam == CreatureTeam.Enemy)
                    {
                        DetectTarget(GameManager.Instance.myFieldCreature);
                    }
                }
                else if (attackType == AttackType.SkillAttack)
                {
                    if (creatureTeam == CreatureTeam.Player)
                    {
                        DetectTarget(GameManager.Instance.enemyFieldCreature);
                    }
                    else if (creatureTeam == CreatureTeam.Enemy)
                    {
                        DetectTarget(GameManager.Instance.myFieldCreature);
                    }
                    skillCoolTimeCheck = true;
                    ChangeState(CreatureState.Attack);
                }
            }
            else
            {
                targetCreature = null;
                targetPos = startPos;
            }
            dir = (targetPos - transform.position).normalized;
        }
        if (attackType == AttackType.NormalAttack)
        {
            if (targetCreature != null)
            {
                destination = targetCreature.transform.position;
            }
            else
            {
                destination = targetPos;
            }
            Vector3 next = Vector3.Lerp(transform.position, destination, 20f * Time.deltaTime);
        
            rigidbody.MovePosition(next);

            if (targetCreature == null && Vector3.Distance(next, targetPos) <= 0.1f)
            {
                transform.position = targetPos;
                attackEnd.Invoke();
                ChangeState(CreatureState.Idle);
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (isAttacking == true)
        {
            if (creatureTeam == CreatureTeam.Player)
            {
                if (other.CompareTag("Enemy"))
                {
                    if (other.GetComponent<IDamageAble>().Collider == targetCreature.collider)
                    {
                        ChangeState(CreatureState.Attack);
                    }
                }
            }
            else if (creatureTeam == CreatureTeam.Enemy)
            {
                if (other.CompareTag("Player"))
                {
                    if (other.GetComponent<IDamageAble>().Collider == targetCreature.collider)
                    {
                        ChangeState(CreatureState.Attack);
                    }
                }
            }
        }
    }
    private void AttackState()
    {
        if (stateEnter == false)
        {
            stateEnter = true;
            animator.Play("Attack");
            Attack();
        }
        if (animatorState.IsName("Attack"))
        { 
            if (animatorState.normalizedTime >= 1f)
            {
                isAttacking = false;
                if (attackType == AttackType.NormalAttack)
                {
                    ChangeState(CreatureState.Move);
                }
                else if(attackType == AttackType.SkillAttack)
                {
                    ChangeState(CreatureState.Idle);
                    targetCreature = null;
                }
            }
        }
    }
    private void DamageState()
    {
        if (stateEnter == false)
        {
            stateEnter = true;
            animator.Play("Damage");
        }
        if (animatorState.IsName("Damage"))
        {
            if (animatorState.normalizedTime >= 1f)
            {
                ChangeState(CreatureState.Idle);
            }
        }
    }
    private void DieState()
    {
        if (stateEnter == false)
        {
            stateEnter = true;
            die = true;
            animator.Play("Die");
        }
    }
    public void ChangeState(CreatureState newState)
    {
        currentState = newState;
        stateEnter = false;
    }
    public virtual void TakeDamage(int damage, bool OnDamage)
    {
        ChangeState(CreatureState.Damage);
        currentHp -= damage;
        float ratio  = currentHp / (float)maxHp;
        ratio = Mathf.Clamp(ratio, 0f, 1f);
        hpBar.SetHpRatio(ratio); 
        if(currentHp <= 0)
        {
            currentHp = 0;
            ChangeState(CreatureState.Die);
        }
    }
    void DetectTarget(List<BaseCreature> fieldCreature)
    {
        int creatureNum = fieldCreature.Count(c=>c.IsDie == false);
        if (creatureNum == 0) return;
        int randNum = Random.Range(0, creatureNum);
        List<BaseCreature> aliveCreature = fieldCreature.Where(c => c.IsDie == false).ToList();
        targetCreature = aliveCreature[randNum];
        if (attackType == AttackType.NormalAttack)
        {
            targetPos = targetCreature.transform.position;
        }
        Vector3 lookDirection = targetCreature.transform.position - transform.position;
        transform.rotation = Quaternion.LookRotation(lookDirection);
    }
    protected virtual void Attack()
    {
        if (targetCreature == null || targetCreature.collider == null) return;
        IDamageAble targetEnemy = CombatSystem.Instance.GetCreatureOrNull(targetCreature.collider);  
        CombatEvent combatEvent = new CombatEvent();
        if (attackType == AttackType.NormalAttack)
        {
            combatEvent.UseEffect = false;
            combatEvent.Damage = attack;

        }
        else if (attackType == AttackType.SkillAttack)
        {
            combatEvent.UseEffect = true;
            combatEvent.EffectName = skillName;
            combatEvent.EffectPosition = targetEnemy.Collider.bounds.center;
            combatEvent.Damage = skillAttack;
            if (creatureTeam == CreatureTeam.Player)
            {
                combatCreatureSlot.StartCoolTime_Coroutine(skillCoolTime, this);
            }
            else if (creatureTeam == CreatureTeam.Enemy)
            {
                StartCoroutine(CoolTime());
            }
        }
        combatEvent.Sender = this;
        combatEvent.Receiver = targetEnemy;
        CombatSystem.Instance.AddCombatEvent(combatEvent);
        
    }

    public IEnumerator CoolTime()
    {
        float elapsedTime = 0f;
        while (elapsedTime < coolTime)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        skillCoolTimeCheck = false;
    }
}
 