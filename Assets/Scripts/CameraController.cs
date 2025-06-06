using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    [Header("Mouse Settings")]
    [SerializeField]
    private float mouseYSensitivity; // Mouse Y Sensitivity
    [SerializeField]
    private float mouseXSensitivity; // Mouse X Sensitivity
    [SerializeField]
    private float mouseYUpperLimit = 3.5f;
    [SerializeField]
    private float mouseYLowerLimit = 0f;

    private float mouseX;
    private float mouseY;

    public bool isStop = false;


    public GameObject playerPosition;

    public Vector3 initialCameraPosition = new Vector3(0, 1.2f, -2f);

    void Start()
    {
        InitCamera();
    }
    void Update()
    {
        if (isStop)
        {
            return;

        }
        else
        {
            CameraMove();
            CameraFollow();
        }

    }
    private void InitCamera()
    {
        transform.position = playerPosition.transform.position + initialCameraPosition;
    }
    private void CameraFollow()
    {
        Vector3 arrangedPlayerPosition = new Vector3(playerPosition.transform.position.x, playerPosition.transform.position.y, playerPosition.transform.position.z) + new Vector3(0, 2f, 0);
        transform.localRotation = Quaternion.LookRotation(arrangedPlayerPosition - transform.position).normalized; // 항상 플레이어를 바라볼 수 있게 해준다. 
    }

    private void CameraMove()
    {
        mouseY -= Input.GetAxisRaw("Mouse Y") * mouseYSensitivity * Time.deltaTime;
        mouseY = Mathf.Clamp(mouseY, -mouseYLowerLimit, mouseYUpperLimit);
        mouseX += Input.GetAxis("Mouse X") * mouseXSensitivity * Time.deltaTime;

        float radius = -3.0f;
        float x = Mathf.Sin(mouseX) * radius;
        float z = Mathf.Cos(mouseX) * radius;
        // transform.position = playerPosition.transform.position + new Vector3(-Mathf.Sin(mouseX) * radius, mouseY, (1- (Mathf.Sin(mouseX) * Mathf.Sin(mouseX))) * radius * Mathf.Cos(mouseX));

        transform.position = playerPosition.transform.position + new Vector3(x, mouseY, z);
    }

    public void SetRestCamera()
    {
        isStop = true;
        StartCoroutine(MoveCamera());
    }

    IEnumerator MoveCamera()
    {
        Vector3 targetPosition = playerPosition.transform.position + new Vector3(0, 2f, 6f);
        Vector3 startPosition = transform.position;


        float time = 0.5f;
        float t = 0;
        while (t < time)
        {
            t += Time.deltaTime;
            transform.position = Vector3.Lerp(startPosition, targetPosition, t / time);
            Quaternion targetRotation = Quaternion.LookRotation(playerPosition.transform.position - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, t / time);
            yield return null;
        }
    }
}
