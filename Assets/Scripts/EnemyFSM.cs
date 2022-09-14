using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyFSM : MonoBehaviour
{
    public enum EnemyState
    {
        Idle, 
        Move,
        Attack,
        Return,
        Damanged,
        Die
    }
    public EnemyState m_state;

    public float findDistance = 8f;
    public float attackDistance = 2f;
    public float moveDistance = 20f;

    public float moveSpeed = 5f;
    public int attackPower = 3;

    Transform player;
    CharacterController cc;

    float currentTime = 0;
    float attackDelay = 2f;

    Vector3 originPos;
    Quaternion originRot;
    public int hp = 15;
    int maxHp = 15;

    public Slider hpSlider;
    Animator anim;
    NavMeshAgent smith;

    //----------------------------------------------------

    private void Start() {
        m_state = EnemyState.Idle;
        player = GameObject.Find("Player").transform;
        cc = GetComponent<CharacterController>();
        originPos = transform.position;
        originRot = transform.rotation;
        anim = transform.GetComponentInChildren<Animator>();
        smith = GetComponent<NavMeshAgent>();
    }

    private void Update() {
        switch(m_state)
        {
            case EnemyState.Idle :
                Idle();
                break;
            case EnemyState.Move :
                Move();
                break;
            case EnemyState.Attack :
                Attack();
                break;
            case EnemyState.Return :
                Return();
                break;
            case EnemyState.Damanged :
                //Damanged();
                break;
            case EnemyState.Die :
                //Die();
                break;
        }

        hpSlider.value = (float)hp / (float)maxHp;
    }

    private void Idle()
    {
        if (Vector3.Distance(transform.position, player.position) < findDistance)
        {
            m_state = EnemyState.Move;
            print("상태 전환 : Idle -> Move");

            anim.SetTrigger("IdleToMove");
        }
    }

    private void Move()
    {
        if (Vector3.Distance(transform.position, originPos) > moveDistance) {
            m_state = EnemyState.Return;
            print("상태 전환 : Move -> Return"); 
            return;
        }

        if (Vector3.Distance(transform.position, player.position) < attackDistance)
        {
            m_state = EnemyState.Attack;
            print("상태 전환 : Move -> Attack");

            currentTime = attackDelay;
            anim.SetTrigger("MoveToAttackDelay");
        }
        else
        {
            /*Vector3 dir = (player.position - transform.position).normalized;
            transform.forward = dir;
            cc.Move(dir * moveSpeed * Time.deltaTime); */

            smith.isStopped = true;
            smith.ResetPath();
            smith.stoppingDistance = attackDistance;
            smith.destination = player.position;
        }
    }

    private void Attack()
    {
        if (Vector3.Distance(transform.position, player.position) >= attackDistance)
        {
            m_state = EnemyState.Move;
            print("상태 전환 : Attack -> Move");

            anim.SetTrigger("AttackToMove");
        }
        else
        {
            currentTime += Time.deltaTime;
            if (currentTime > attackDelay)
            {
                //player.GetComponent<PlayerMove>().DamageAction(attackPower);
                print("공격");
                currentTime = 0;

                anim.SetTrigger("StartAttack");
            }
        }
    }

    public void AttackAction()
    {
        player.GetComponent<PlayerMove>().DamageAction(attackPower);
    }

    private void Return()
    {
        if (Vector3.Distance(transform.position, originPos) <= 0.1f)
        {
            smith.isStopped = true;
            smith.ResetPath();

            transform.position = originPos;
            transform.rotation = originRot;
            hp = maxHp;

            m_state = EnemyState.Idle;
            print("상태 전환 : Return -> Idle");

            anim.SetTrigger("MoveToIdle");
        }
        else
        {
            /* Vector3 dir = (originPos - transform.position).normalized;
            transform.forward = dir;
            cc.Move(dir * moveSpeed * Time.deltaTime); */

            smith.destination = originPos;
            smith.stoppingDistance = 0;
        }
    }

    private void Damanged()
    {
        StartCoroutine(DamageProcess());
    }

    IEnumerator DamageProcess()
    {
        //0.5초 기다리고
        yield return new WaitForSeconds(1f);

        //Move로 상태 변경
        m_state = EnemyState.Move;
        print("상태 전환 : Damanged -> Move");

        anim.SetTrigger("IdleToMove");  //<== 
    }

    public void HitEnemy(int hitPower)
    {
        if(m_state == EnemyState.Damanged || m_state == EnemyState.Die || m_state == EnemyState.Return)
        {
            return;
        }

        hp -= hitPower;
        smith.isStopped = true;
        smith.ResetPath();

        if (hp > 0)
        {
            m_state = EnemyState.Damanged;
            print("상태 전환 : Any -> Damanged");

            anim.SetTrigger("Damaged");
            Damanged();
        }
        else
        {
            m_state = EnemyState.Die;
            print("상태 전환 : Any -> Die");

            anim.SetTrigger("Die");
            Die();
        }
    }

    private void Die()
    {
        StopAllCoroutines();

        StartCoroutine(DieProcess());
    }

    IEnumerator DieProcess()
    {
        cc.enabled = false;

        yield return new WaitForSeconds(2);
        print("소멸");
        Destroy(gameObject);
    }
}
