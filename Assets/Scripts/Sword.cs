using System;
using System.Collections;
using UnityEngine;

public class Sword : MonoBehaviour
{
    public int damage;
    public float attackSpeed;
    public BoxCollider attackArea;

    private PlayerController _playerController;
    private Enemy _enemy;

    public void Use()
    {
        StopCoroutine("Swing");
        StartCoroutine("Swing");
    }

    private IEnumerator Swing()
    {
        _playerController = GetComponentInParent<PlayerController>();
        
        yield return new WaitForSeconds(0.1f);
        attackArea.enabled = true;
        _playerController.isAttacking = true;
        

        yield return new WaitForSeconds(0.8f);
        attackArea.enabled = false;
        _playerController.isAttacking = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            _enemy = other.GetComponent<Enemy>();
            if (_enemy != null)
            {
                _enemy.hp -= damage;
            }
        }
    }
}
