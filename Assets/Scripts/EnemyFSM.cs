using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyFSM : MonoBehaviour
{
    enum EnemyState
    {
        Idle, 
        Move,
        Attack,
        Return,
        Damanged,
        Die
    }
    EnemyState m_state;

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
    public int hp = 15;
    int maxHp = 15;

    public Slider hpSlider;

    //----------------------------------------------------

    private void Start() {
        m_state = EnemyState.Idle;
        player = GameObject.Find("Player").transform;
        cc = GetComponent<CharacterController>();
        originPos = transform.position;
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
            print("���� ��ȯ : Idle -> Move");
        }
    }

    private void Move()
    {
        if (Vector3.Distance(transform.position, originPos) > moveDistance) {
            m_state = EnemyState.Return;
            print("���� ��ȯ : Move -> Return"); 
            return;
        }

        if (Vector3.Distance(transform.position, player.position) < attackDistance)
        {
            m_state = EnemyState.Attack;
            print("���� ��ȯ : Move -> Attack");

            currentTime = attackDelay;
        }
        else
        {
            Vector3 dir = (player.position - transform.position).normalized;
            cc.Move(dir * moveSpeed * Time.deltaTime);
        }
    }

    private void Attack()
    {
        if (Vector3.Distance(transform.position, player.position) >= attackDistance)
        {
            m_state = EnemyState.Move;
            print("���� ��ȯ : Attack -> Move");
        }
        else
        {
            currentTime += Time.deltaTime;
            if (currentTime > attackDelay)
            {
                player.GetComponent<PlayerMove>().DamageAction(attackPower);
                print("����");
                currentTime = 0;
            }
        }
    }

    private void Return()
    {
        if (Vector3.Distance(transform.position, originPos) < 0.1f)
        {
            m_state = EnemyState.Idle;
            print("���� ��ȯ : Return -> Idle");
        }
        else
        {
            Vector3 dir = (originPos - transform.position).normalized;
            cc.Move(dir * moveSpeed * Time.deltaTime);
        }
    }

    private void Damanged()
    {
        StartCoroutine(DamageProcess());
    }

    IEnumerator DamageProcess()
    {
        //0.5�� ��ٸ���
        yield return new WaitForSeconds(0.5f);

        //Move�� ���� ����
        m_state = EnemyState.Move;
        print("���� ��ȯ : Damanged -> Move");
    }

    public void HitEnemy(int hitPower)
    {
        if(m_state == EnemyState.Damanged || m_state == EnemyState.Die || m_state == EnemyState.Return)
        {
            return;
        }

        hp -= hitPower;

        if (hp > 0)
        {
            m_state = EnemyState.Damanged;
            print("���� ��ȯ : Any -> Damanged");
            Damanged();
        }
        else
        {
            m_state = EnemyState.Die;
            print("���� ��ȯ : Any -> Die");
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
        print("�Ҹ�");
        Destroy(gameObject);
    }
}
