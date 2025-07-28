using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatManager : MonoBehaviour
{
    public static StatManager instance; 
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }

    public int statPoints = 0;
    public int hpPoint = 0;
    public int attackPoint = 0;

    PlayerController playerController;

    private void Start()
    {
        playerController = GameManager.Instance.player.GetComponent<PlayerController>();
    }
    public void OnClickHPUp()
    {
        if(statPoints > 0)
        {
            playerController.maxHp += hpPoint * 5;
            statPoints--;
            hpPoint++;
            UIManager.Instance.UpdateStatPoints();
        }
    }
    public void OnClickHPDown()
    {
        if (hpPoint > 0) 
        {
            playerController.maxHp -= hpPoint * 5;
            statPoints++;
            hpPoint--;
            UIManager.Instance.UpdateStatPoints();
            
        }
    }
    public void OnClickAttackUp()
    {
        if (statPoints > 0)
        {
            playerController.sword.damage += attackPoint;
            statPoints--;
            attackPoint++;
            UIManager.Instance.UpdateStatPoints();
        }
    }
    public void OnClickAttackDown()
    {
        if (attackPoint > 0)
        {
            playerController.sword.damage -= attackPoint;
            statPoints++;
            attackPoint--;
            UIManager.Instance.UpdateStatPoints();

        }
    }
}
