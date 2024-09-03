using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float turnSpeed = 5f;
    private Vector3 movement;
    
    Rigidbody rb;
    Animator anim;
    
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    void Move()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // 벡터의 정규화를 통해서 모든 방향의 이동속도를 동일하게 만든다.
        movement = new Vector3(horizontal, 0, vertical).normalized;

        // 이동
        transform.position += movement * moveSpeed * Time.deltaTime;
        
        // 애니메이션
        anim.SetBool("Walking", movement != Vector3.zero);
        
        // 회전
        if (movement != Vector3.zero) // 키 입력값이 존재할 때
        {
            Quaternion toRotation = Quaternion.LookRotation(movement, Vector3.up); // 해당 방향을 바라봄
            transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, turnSpeed * Time.deltaTime); // 부드러운 회전
        }
    }
}
