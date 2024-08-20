using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class CameraManager : MonoBehaviour
{

    public float mouseSensitivity = 400f;

    public GameObject player;

    private float mouseX;
    private float mouseY;



    
    void Start()
    {
        
    }

    
    void Update()
    {
        CameraRotate();
        CameraMove();
    }

    private void CameraRotate()
    {
        mouseX += Input.GetAxisRaw("Mouse X") * mouseSensitivity * Time.deltaTime;

        mouseY -= Input.GetAxisRaw("Mouse Y") * mouseSensitivity * Time.deltaTime;

        mouseY = Mathf.Clamp(mouseY, -90f, 90f); //Clamp를 통해 최소값 최대값을 넘지 않도록함

        transform.localRotation = Quaternion.Euler(mouseY, mouseX, 0f);// 각 축을 한꺼번에 계산
    }

    private void CameraMove()
    {
        transform.position = player.transform.position + new Vector3(0, 2f, -2f);
    }
}
