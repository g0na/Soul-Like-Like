using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Border : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("ENTER!!");
        }
    }
    private void OnTriggerExit(Collider other)
    {
        
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        // Gizmos.DrawCube(this.transform.position, this.transform.lossyScale);
    }
}
