using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    public int max = 100;
    public int hp = 100;
    public Transform player;
    public float moveSpeed = 2.0f;
    public float rotationSpeed = 3.0f;
    public float attackRange = 1000.0f; // 공격 범위

    int bossDamage = 20;
    private bool isAlive = true;
    private bool isAttacking = false;

    Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
            player = playerObject.transform;

        anim.SetBool("Idle", true);
    }

    void Update()
    {
        if (!isAlive)
            return;

        if (hp <= 0)
        {
            isAlive = false;
            anim.SetBool("Idle", false);
            anim.SetTrigger("Death");
            //GetComponent<Collider>().enabled = false; 
            UIManager.Instance.HideBossHpBar();
            Destroy(this.gameObject, 4.5f);
            return;
        }

        if (player != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            if (distanceToPlayer > attackRange)
            {
                // 플레이어가 공격 범위 밖에 있으면 추적
                MoveTowardsPlayer();
            }
            else
            {
                // 플레이어가 공격 범위 안에 있으면 멈추고 공격 준비
                StopMoving();
            }
        }
    }

    void MoveTowardsPlayer()
    {
        if (isAttacking) return;

        anim.SetBool("Idle", false);
        anim.SetBool("Walking", true);

        // 플레이어 방향으로 부드럽게 회전
        Vector3 direction = (player.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);

        // 앞으로 이동
        transform.position += transform.forward * moveSpeed * Time.deltaTime;
    }

    void StopMoving()
    {
        if (isAttacking) return;

        anim.SetBool("Idle", true);
        anim.SetBool("Walking", false);

        // 공격 애니메이션이 재생 중이 아닐 때만 공격 시도
        if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Attack1") && 
            !anim.GetCurrentAnimatorStateInfo(0).IsName("Attack2"))
        {
            StartCoroutine(Attack());
        }
    }

    IEnumerator Attack()
    {
        isAttacking = true;
        anim.SetBool("Idle", false);

        // 랜덤하게 Attack1 또는 Attack2
        if (UnityEngine.Random.Range(0, 2) == 0)
        {
            anim.SetBool("Walking", false);
            anim.SetTrigger("Attack1");
        }
        else
        {
            anim.SetBool("Walking", false);
            anim.SetTrigger("Attack2");
        }

        // 공격 애니메이션이 끝날 때까지 대기
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f);

        isAttacking = false;
        anim.SetBool("Idle", true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerController>().Get_Damage(20, this.gameObject);
            Debug.Log("Player Hit");
        }
    }
    public void Hit(int dmg)
    {
        if (!isAlive) 
            return;

        this.hp -= dmg;
        UIManager.Instance.ShowDamageText(dmg);
        UIManager.Instance.UpdateBossHpBar((float)hp/max);
    }
}
