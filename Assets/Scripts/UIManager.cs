using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{


    public TMPro.TextMeshProUGUI currentRegionText;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        currentRegionText.text = GameManager.Instance.currentRegion;
    }
}
