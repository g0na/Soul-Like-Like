using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Border : MonoBehaviour
{

    public GameObject[] borderPart = new GameObject[2];

    private bool isBorder = false;
    private int borderCount = 0;
    private string lastExitBorderName;


    public void ContactPlayer()
    {
        borderCount++;

        if(borderCount == 2)
        {
            isBorder = true;
        }
    }

    public void UntactPlayer(string regionName)
    {
        borderCount--;
        lastExitBorderName = regionName;
        if (borderCount == 0)
        {
            GameManager.Instance.ChangeRegion(lastExitBorderName);
            isBorder = false;
        }        
    }
}
