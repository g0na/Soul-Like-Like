using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    private static UIManager instance = null;
    public TMPro.TextMeshProUGUI currentRegionText;

    private bool isShowingRegionText = false;

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
        if (!isShowingRegionText)
        {
            isShowingRegionText = true;
            StartCoroutine(ShowRegionName());
        }
    }

    IEnumerator ShowRegionName()
    {
        float transp = 0f;
        currentRegionText.text = GameManager.Instance.currentRegion;
        currentRegionText.color = new Color(1f, 1f, 1f, transp);
        while (true)
        {
            if(transp >= 1f)
            {
                Debug.Log("end");
                isShowingRegionText = false;
                break;
            }
            yield return new WaitForSeconds(0.01f);
            transp += 0.01f;
            currentRegionText.color = new Color(1f, 1f, 1f, transp);
            Debug.Log(transp);

        }
        yield return null;

    }

    public void ShowDamageText(int dmg)
    {
        Debug.Log(dmg);
    }
}
