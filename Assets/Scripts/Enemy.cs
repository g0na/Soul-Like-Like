using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int hp;

    public SphereCollider enemyCognitionRange;


    private void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("!");
    }
}
