using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private static UIManager instance = null;
    public TMPro.TextMeshProUGUI currentRegionText;
    public Slider hpBar;

    public GameObject deadPanel;
    public GameObject deadText;
    public GameObject continueText;

    private bool isShowingRegionText = false;

    public GameObject restPanel;

    public GameObject bossHpBar;

    [Header("Stat Points")]
    public TMPro.TextMeshProUGUI statPointText;
    public TMPro.TextMeshProUGUI hpPointText;
    public TMPro.TextMeshProUGUI attackPointText;


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

    public void ChangeHealth()
    {
        hpBar.value = (float)GameManager.Instance.player.GetComponent<PlayerController>().currentHp / (float)GameManager.Instance.player.GetComponent<PlayerController>().maxHp;
        Debug.Log(hpBar.value);
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
            if (transp >= 1f)
            {
                StartCoroutine(FadeOutRegionName());
                break;
            }
            yield return new WaitForSeconds(0.01f);
            transp += Time.deltaTime;
            currentRegionText.color = new Color(1f, 1f, 1f, transp);

        }
        yield return null;
    }

    IEnumerator FadeOutRegionName()
    {
        float transp = 1f;
        while (true)
        {
            if (transp <= 0f)
            {
                isShowingRegionText = false;
                break;
            }
            yield return new WaitForSeconds(0.01f);
            transp -= Time.deltaTime;
            currentRegionText.color = new Color(1f, 1f, 1f, transp);
        }
        yield return null;
    }


    public void ShowDamageText(int dmg)
    {
        Debug.Log(dmg);
    }

    public void ShowDeadPanel()
    {
        deadPanel.SetActive(true);
        deadPanel.GetComponent<Image>().color = new Color(0f, 0f, 0f, 0.0f);
        StartCoroutine(ShowDeadPanelCoroutine());
    }

    IEnumerator ShowDeadPanelCoroutine()
    {
        float endAlpha = 0.8f;
        float tempAlpha = 0f;
        while (tempAlpha < endAlpha)
        {
            deadPanel.GetComponent<Image>().color = new Color(0f, 0f, 0f, tempAlpha);
            tempAlpha += Time.deltaTime;
            Debug.Log(tempAlpha);
            yield return null;
        }
        StartCoroutine(ShowDeadText());
        

        Debug.Log("End Fade");
    }

    IEnumerator ShowDeadText()
    {
        float endAlpha = 1f;
        float tempAlpha = 0f;

        deadText.SetActive(true);
        continueText.SetActive(true);

        deadText.GetComponent<TextMeshProUGUI>().color = new Color(1f, 0f, 0f, tempAlpha);
        continueText.GetComponent<TextMeshProUGUI>().color = new Color(1f, 1f, 1f, tempAlpha);
        while (tempAlpha < endAlpha)
        {
            deadText.GetComponent<TextMeshProUGUI>().color = new Color(1f, 0f, 0f, tempAlpha);
            continueText.GetComponent<TextMeshProUGUI>().color = new Color(1f, 1f, 1f, tempAlpha);
            tempAlpha += Time.deltaTime;
            yield return null;
        }
        deadPanel.GetComponent<Button>().onClick.AddListener(OnClickDeadPanel);
    }

    public void OnClickDeadPanel()
    {
        deadPanel.SetActive(false);
        deadText.SetActive(false);
        continueText.SetActive(false);
        GameManager.Instance.ReSpawn();
        deadPanel.GetComponent<Button>().onClick.RemoveListener(OnClickDeadPanel);
    }

    public void ShowRestPanel()
    {
        restPanel.SetActive(true);
    }
    public void HideRestPanel()
    {
        restPanel.SetActive(false);
    }

    public void OnClickEndRestButton()
    {
        GameManager.Instance.EndRest();
    }

    public void UpdateStatPoints()
    {
        statPointText.text = "Stat Point : " + StatManager.instance.statPoints.ToString();
        hpPointText.text = "HP : " + StatManager.instance.hpPoint.ToString();
        attackPointText.text = "ATK : " + StatManager.instance.attackPoint.ToString();
}

    public void ShowBossHpBar()
    {
        bossHpBar.SetActive(true);
    }

    public void HideBossHpBar()
    {
        bossHpBar.SetActive(false);
    }

    public void UpdateBossHpBar(float hp)
    {
        bossHpBar.GetComponent<Slider>().value = hp;
    }
}
