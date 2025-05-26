using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    private static GameManager instance = null;
    public GameObject mainCamera;
    public GameObject player;
    [SerializeField]
    public string currentRegion;

    public Transform spawnPotint;

    public bool isResting;

    void Awake()
    {
        if(null == instance)
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
        currentRegion = "plain";
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static GameManager Instance
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
    public void ChangeRegion(string regionName)
    {
        currentRegion = regionName;
        UIManager.Instance.ChangeRegionName();
    }

    public void ReSpawn()
    {
        player.transform.position = spawnPotint.position;
        player.GetComponent<PlayerController>().currentHp = player.GetComponent<PlayerController>().maxHp;
        player.GetComponent<PlayerController>().isAlive = true;
        player.GetComponent<PlayerController>().anim.Play("Idle");
        UIManager.Instance.ChangeHealth();
    }


    public void Rest()
    {
        mainCamera.GetComponent<CameraController>().SetRestCamera();
        UIManager.Instance.ShowRestPanel();
        spawnPotint.position = player.transform.position; // 현재 플레이어 위치를 재생성 지점으로 설정
        // Time.timeScale = 0f; // 게임 일시 정지
        isResting = true;
    }

    public void EndRest()
    {
        mainCamera.GetComponent<CameraController>().isStop = false;
        UIManager.Instance.HideRestPanel();
        isResting = false;
        player.GetComponent<PlayerController>().anim.SetBool("Sitting", false);
    }

   

}
