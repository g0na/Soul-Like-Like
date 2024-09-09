using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    public float mouseSensitivity;

    private float mouseX;
    private float mouseY;

    private float mouseYUpperLimit = 3.5f;
    private float mouseYLowerLimit = 0f;

    public GameObject playerCenter;



    void Start()
    {
        InitCamera();
    }

    
    void Update()
    {
        CameraMove();
        CameraFollow();
    }
    private void InitCamera()
    {
        transform.position = playerCenter.transform.position + new Vector3(0, 2f, -2f);
    }
    private void CameraFollow()
    {
        Vector3 arrangedPlayerPosition = new Vector3(playerCenter.transform.position.x, playerCenter.transform.position.y, playerCenter.transform.position.z);
        transform.localRotation = Quaternion.LookRotation(arrangedPlayerPosition - transform.position).normalized;
    }

    private void CameraMove()
    {
        mouseY -= Input.GetAxisRaw("Mouse Y") * mouseSensitivity * Time.deltaTime;
        mouseY = Mathf.Clamp(mouseY, -mouseYLowerLimit, mouseYUpperLimit);
        mouseX -= Input.GetAxis("Mouse X") * 0.03f;

        float radius = - 3.0f;
        transform.position = playerCenter.transform.position + new Vector3(-Mathf.Sin(mouseX) * radius, mouseY, (1- (Mathf.Sin(mouseX) * Mathf.Sin(mouseX))) * radius * Mathf.Cos(mouseX));
    }
}
