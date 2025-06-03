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

    private bool isStunned = false;
    public GameObject stunnedObject;

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
        if (isStunned)
        { 
        }
        else
        {
            CloseEnough();
            Follow();
        }
        CheckDeath();
    }



    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player Detected");
            isPlayerCloseEnough = true;
            
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

    private void CloseEnough()
    {
        if (isPlayerCloseEnough && distanceToPlayer <= attackRange)
        {
            StartCoroutine(OpenParryWindow());
            anim.SetTrigger("attack");
        }
    }

    private void Attack()
    {
        Vector3 attackOrigin = transform.position + transform.forward * forwardOffset + Vector3.up * verticalOffset;
        Collider[] hitColliders = Physics.OverlapSphere(attackOrigin, attackCheckRadius);

        foreach (Collider col in hitColliders)
        {
            if (col.CompareTag("Player"))
            {
                col.GetComponent<PlayerController>().Get_Damage(enemyDamage, this.gameObject);
                return;
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

    void Follow()
    {
        if (isPlayerCloseEnough)
        {
            anim.SetBool("walk", true);
            this.transform.LookAt(GameManager.Instance.player.transform);
            this.transform.position += this.transform.forward * moveSpeed * Time.deltaTime;
            distanceToPlayer = Vector3.Distance(transform.position, GameManager.Instance.player.transform.position);
        }
    }

    public void Hit(int dmg)
    {
        try
        {
            anim.Play("Pose");
        }
        catch
        {
            // 애니메이션이 없을 경우 무시하고 넘어감
        }
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
            var animParameters = anim.parameters;
            foreach (var param in anim.parameters)
            {
                if(param.name == "isDead")
                {
                    anim.SetTrigger("isDead");
                }
            }            
            Destroy(this.gameObject, 2f);
        }
    }

    public void GetCounterAttack()
    {
        try
        {
            anim.Play("Pose");
        }
        catch
        {
            // 애니메이션이 없을 경우 무시하고 넘어감
        }
        StartCoroutine(StartStun());
    }
    IEnumerator StartStun()
    {
        isStunned = true;
        stunnedObject.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        isStunned = false;
        stunnedObject.SetActive(false);
    }
}
