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

    private void OnCollisionEnter(Collision collision)
    {
        // Debug.Log("!");
    }

    public void Hit(int dmg)
    {
        Debug.Log("!");
        

        this.hp -= dmg;
        UIManager.Instance.ShowDamageText(dmg);
        if (hp < 0)
        {
            Destroy(this.gameObject);
        }
    }
}
