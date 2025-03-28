using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    [Header("Mouse Settings")]
    [SerializeField]
    private float mouseYSensitivity;
    [SerializeField]
    private float mouseXSensitivity;
    [SerializeField]
    private float mouseYUpperLimit = 3.5f;
    [SerializeField]
    private float mouseYLowerLimit = 0f;

    private float mouseX;
    private float mouseY;

    public GameObject playerPosition;

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
        transform.position = playerPosition.transform.position + new Vector3(0, 1.2f, -2f);
    }
    private void CameraFollow()
    {
        Vector3 arrangedPlayerPosition = new Vector3(playerPosition.transform.position.x, playerPosition.transform.position.y, playerPosition.transform.position.z) + new Vector3(0, 2f, 0);
        transform.localRotation = Quaternion.LookRotation(arrangedPlayerPosition - transform.position).normalized;
    }

    private void CameraMove()
    {
        mouseY -= Input.GetAxisRaw("Mouse Y") * mouseYSensitivity * Time.deltaTime;
        mouseY = Mathf.Clamp(mouseY, -mouseYLowerLimit, mouseYUpperLimit);
        mouseX -= Input.GetAxis("Mouse X") * mouseXSensitivity * 0.01f;

        float radius = - 3.0f;
        transform.position = playerPosition.transform.position + new Vector3(-Mathf.Sin(mouseX) * radius, mouseY, (1- (Mathf.Sin(mouseX) * Mathf.Sin(mouseX))) * radius * Mathf.Cos(mouseX));
    }
}
