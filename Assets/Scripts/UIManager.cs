using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    private static UIManager instance = null;

    public TMPro.TextMeshProUGUI currentRegionText;

    // Start is called before the first frame update

    void Awake()
    {
        if (null == instance)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }

    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static UIManager Instance
    {
        get
        {
            if (null == instance)
            {
                return null;
            }
            return instance;
        }
    }


    public void ChangeRegionName()
    {
        currentRegionText.text = GameManager.Instance.currentRegion;
        StartCoroutine(ShowRegionName());

    }

    IEnumerator ShowRegionName()
    {
        int transp = 0;
        while (true)
        {
            if (transp == 100)
            {
                break;
            }
            Debug.Log(transp);
            transp += 1;
        }        
        yield return null;
    }

    public void ShowDamageText(int dmg)
    {
        Debug.Log(dmg);
    }
}
