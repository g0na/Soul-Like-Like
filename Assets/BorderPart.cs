using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BorderPart : MonoBehaviour
{
    public string regionBorderName;

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
        if (other.tag == "Player")
        {
            transform.GetComponentInParent<Border>().ContactPlayer();
        }
        
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            transform.GetComponentInParent<Border>().UntactPlayer(regionBorderName);
        }
    }
}
