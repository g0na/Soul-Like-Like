using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonFire : MonoBehaviour
{

    public GameObject interactHint; 

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerController>().isBonFire = true;
            interactHint.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerController>().isBonFire = false;
            interactHint.SetActive(false);
        }
    }

}
