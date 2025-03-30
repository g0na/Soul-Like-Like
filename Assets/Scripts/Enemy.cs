using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int hp;
    private bool isInvincible = false;

    public SphereCollider enemyCognitionRange;


    private void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player Detected");
            // TODO: Attack Player
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
