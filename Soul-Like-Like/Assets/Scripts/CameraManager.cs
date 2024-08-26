using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class CameraManager : MonoBehaviour
{

    public float mouseSensitivity;

    public GameObject player;

    private float mouseX;
    private float mouseY;

    private float mouseYUpperLimit = 3.5f;
    private float mouseYLowerLimit = 0f;

    
    void Start()
    {
        InitCamera();
    }

    
    void Update()
    {
        // CameraRotate();
        CameraMove();
        CameraFollow();
    }
    private void InitCamera()
    {
        transform.position = player.transform.position + new Vector3(0, 2f, -2f);

    }
    private void CameraRotate()
    {


        mouseX += Input.GetAxisRaw("Mouse X") * mouseSensitivity * Time.deltaTime;
        mouseY -= Input.GetAxisRaw("Mouse Y") * mouseSensitivity * Time.deltaTime;

        mouseY = Mathf.Clamp(mouseY, -mouseYLowerLimit, mouseYUpperLimit); //Clamp를 통해 최소값 최대값을 넘지 않도록함
        transform.localRotation = Quaternion.Euler(mouseY, mouseX, 0f);// 각 축을 한꺼번에 계산
    }

    private void CameraFollow()
    {
        Vector3 arrangedPlayerPosition = new Vector3(player.transform.position.x, player.transform.position.y + 2f, player.transform.position.z);
        transform.localRotation = Quaternion.LookRotation(arrangedPlayerPosition - transform.position).normalized;
    }

    private void CameraMove()
    {
        mouseY -= Input.GetAxisRaw("Mouse Y") * mouseSensitivity * Time.deltaTime;
        mouseY = Mathf.Clamp(mouseY, -mouseYLowerLimit, mouseYUpperLimit);

        mouseX = Mathf.Sin(Input.GetAxisRaw("Mouse X") * 0.01f) * 360f;
        Debug.Log(mouseX);


        transform.position = player.transform.position + new Vector3(-mouseX, mouseY, 2f);
    }
}
