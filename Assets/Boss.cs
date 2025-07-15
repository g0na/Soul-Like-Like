using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    public int hp = 10;
    public Transform player;
    public float moveSpeed = 2.0f;
    public float rotationSpeed = 3.0f;
    public float attackRange = 1000.0f; // 공격 범위

    int bossDamage = 20;
    private bool isAlive = true;

    Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        else
        {
            Debug.LogError("Player not found! Make sure the player has the 'Player' tag.");
        }
    }

    void Update()
    {
        if (!isAlive)
            return;

        if (hp <= 0)
        {
            isAlive = false;
            anim.SetTrigger("Death");
            GetComponent<Collider>().enabled = false; 
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
        // 플레이어 방향으로 부드럽게 회전
        Vector3 direction = (player.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);

        // 앞으로 이동
        transform.position += transform.forward * moveSpeed * Time.deltaTime;
    }

    void StopMoving()
    {
        // 이동을 멈춤 (필요에 따라 추가적인 로직 구현)
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player Hit");
        }
    }
    public void Hit(int dmg)
    {
        if (!isAlive) return;
        this.hp -= dmg;
        UIManager.Instance.ShowDamageText(dmg);
    }
}
