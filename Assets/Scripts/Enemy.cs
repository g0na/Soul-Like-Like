using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int hp;
    private bool isInvincible = false;

    public SphereCollider enemyCognitionRange;

    private float moveSpeed = 0.4f;

    int enemyDamage = 10;
    bool isPlayerCloseEnough = false;
    float attackRange = 1.1f;
    public float distanceToPlayer;

    private bool isDead = false;

    // [HideInInspector]
    public bool isParriable = false;

    Animator anim;

    // Attack
    float attackCheckRadius = 0.4f;
    [SerializeField] private float forwardOffset = 0.4f;
    [SerializeField] private float verticalOffset = 1.0f;


    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        Attack();
        CheckDeath();
    }



    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player Detected");
            isPlayerCloseEnough = true;
            StartCoroutine(Follow());
            anim.SetBool("walk", true);
            // TODO: Attack Player
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player Out of Range");
            isPlayerCloseEnough = false;
        }
    }

    private void Attack()
    {
        if (isPlayerCloseEnough && distanceToPlayer <= attackRange)
        {
            StartCoroutine(OpenParryWindow());
            anim.SetTrigger("attack");

            Vector3 attackOrigin = transform.position + transform.forward * forwardOffset + Vector3.up * verticalOffset;
            Collider[] hitColliders = Physics.OverlapSphere(attackOrigin, attackCheckRadius);

            foreach (Collider col in hitColliders)
            {
                if (col.CompareTag("Player"))
                {
                    col.GetComponent<PlayerController>().Get_Damage(enemyDamage, this.gameObject);
                    // Debug.Log("플레이어 공격");
                }
            }
        }
    }

    IEnumerator OpenParryWindow()
    {
        if (isParriable)
        {
            
        }
        else
        {
            isParriable = true;
            yield return new WaitForSeconds(0.2f);
            isParriable = false;
        }
    }

    IEnumerator Follow()
    {
        while (isPlayerCloseEnough)
        {
            this.transform.LookAt(GameManager.Instance.player.transform);
            this.transform.position += this.transform.forward * moveSpeed * Time.deltaTime;
            distanceToPlayer = Vector3.Distance(transform.position, GameManager.Instance.player.transform.position);
            yield return new WaitForFixedUpdate();
        }
    }

    public void Hit(int dmg)
    {
        this.hp -= dmg;
        UIManager.Instance.ShowDamageText(dmg);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 attackOrigin = transform.position + transform.forward * forwardOffset + Vector3.up * verticalOffset;
        Gizmos.DrawWireSphere(attackOrigin, attackCheckRadius);
    }

    private void CheckDeath()
    {
        if (hp <= 0)
        {
            anim.SetTrigger("isDead");
            Destroy(this.gameObject, 2f);
        }
    }
}
