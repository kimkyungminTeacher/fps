using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public float moveSpeed = 5f;

    Transform player;
    CharacterController cc;

    float currentTime = 0;
    float attackDelay = 2f;
    //----------------------------------------------------

    private void Start() {
        m_state = EnemyState.Idle;
        player = GameObject.Find("Player").transform;
        cc = GetComponent<CharacterController>();
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
                Damanged();
                break;
            case EnemyState.Die :
                Die();
                break;
        }
    }

    private void Idle()
    {
        if (Vector3.Distance(transform.position, player.position) < findDistance)
        {
            m_state = EnemyState.Move;
            print("상태 전환 : Idle -> Move");
        }
    }

    private void Move()
    {
        if (Vector3.Distance(transform.position, player.position) < attackDistance)
        {
            m_state = EnemyState.Attack;
            print("상태 전환 : Move -> Attack");

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
            print("상태 전환 : Attack -> Move");
        }
        else
        {
            currentTime += Time.deltaTime;
            if (currentTime > attackDelay)
            {
                print("공격!");
                currentTime = 0;
            }
        }
    }


    private void Die()
    {
        throw new NotImplementedException();
    }

    private void Damanged()
    {
        throw new NotImplementedException();
    }

    private void Return()
    {
        throw new NotImplementedException();
    }


}
