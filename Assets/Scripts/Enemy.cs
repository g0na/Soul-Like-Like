using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int hp;
    private bool isInvincible = false;

    public SphereCollider enemyCognitionRange;

    private float moveSpeed = 0.4f;

    bool isPlayerCloseEnough = false;
    float attackRange = 1.1f;
    public float distanceToPlayer;

    Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        Attack();
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
            anim.SetTrigger("attack");
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
        if (hp < 0)
        {
            Destroy(this.gameObject);
        }
    }
}
