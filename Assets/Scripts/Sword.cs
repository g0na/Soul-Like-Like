using System;
using System.Collections;
using UnityEngine;

public class Sword : MonoBehaviour
{
    public int damage;
    public BoxCollider attackArea;

    private PlayerController _playerController;
    private Enemy _enemy;

    bool isHit = false;

    public void Use()
    {
        AttackReset();
        StopCoroutine("Swing");
        StartCoroutine("Swing");
    }

    private void AttackReset()
    {
        isHit = false;
    }

    private IEnumerator Swing()
    {
        _playerController = GetComponentInParent<PlayerController>();
        
        yield return new WaitForSeconds(0.1f);
        attackArea.enabled = true;
        

        yield return new WaitForSeconds(0.8f);
        attackArea.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            _enemy = other.GetComponent<Enemy>();
            if (_enemy != null && !isHit)
            {
                _enemy.Hit(damage);
                isHit = true;
            }
        }
    }
}
