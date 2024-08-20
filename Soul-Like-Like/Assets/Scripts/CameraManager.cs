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

        mouseY = Mathf.Clamp(mouseY, -90f, 90f); //Clamp�� ���� �ּҰ� �ִ밪�� ���� �ʵ�����

        transform.localRotation = Quaternion.Euler(mouseY, mouseX, 0f);// �� ���� �Ѳ����� ���
    }

    private void CameraMove()
    {
        transform.position = player.transform.position + new Vector3(0, 2f, -2f);
    }
}
