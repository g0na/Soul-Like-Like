using System;
using System.Collections;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class PlayerController : MonoBehaviour
{
    private float moveSpeed;
    private float turnSpeed;

    public float dodgeForce;

    [SerializeField]
    private bool isGrounded;
    [SerializeField]
    private bool isJumping;
    [SerializeField]
    private bool isDodging;
    [SerializeField] 
    public Transform groundCheck;

    private RaycastHit slopeHit;
    public float groundCheckDistance;
    public float maxSlopeAngle = 60f;
    public float jumpForce;
    public LayerMask Ground;

    private Vector3 movement;
    private Vector3 movementVertical;
    private Vector3 movementHorizontal;
    
    Rigidbody rb;
    Animator anim;

    public GameObject Camera;
    
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        GroundCheck();
        if (!isDodging)
        {
            Move();
        }
        Jump();
        Block();
        Dodge();
    }

    void Move()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        movementVertical = Vector3.Normalize(new Vector3(this.transform.position.x - Camera.transform.position.x, 0, this.transform.position.z - Camera.transform.position.z)) * vertical;

        Vector3 arrangedTransformPosition = new Vector3(this.transform.position.x, 0, this.transform.position.z);
        Vector3 arrangedCameraPosition = new Vector3(Camera.transform.position.x, 0, Camera.transform.position.z);

        movementHorizontal = Vector3.Normalize(Vector3.Cross(arrangedCameraPosition - arrangedTransformPosition, this.transform.position - arrangedTransformPosition )) * horizontal;

        bool isOnSlope = IsOnSlope();
        
        movement = movementVertical + movementHorizontal;

        if (isJumping)
        {
            moveSpeed = 1f;
            turnSpeed = 2.5f;
        }
        else
        {
            moveSpeed = 5f;
            turnSpeed = 50f;
        }

        if (isGrounded && isOnSlope)
        {
            movement = SlopeDirection(movement);
            //rb.useGravity = false;
        }
        else
        {
            movement = movementVertical + movementHorizontal;
            rb.useGravity = true;
        }
        
        transform.position += movement * moveSpeed * Time.deltaTime;

        // 이동 애니메이션
        if (Input.GetMouseButton(1) && movement != Vector3.zero) // 우클릭이 눌려있을 때
        {
            anim.SetBool("Running", false); // Running 애니메이션 비활성화
            anim.SetBool("BlockingRun", true); // 이동 중이면 BlockingRun 애니메이션 실행
        }
        else // 우클릭이 눌려있지 않을 때
        {
            anim.SetBool("BlockingRun", false); // BlockingRun 애니메이션 비활성화
            anim.SetBool("Running", movement != Vector3.zero); // 이동 중이면 Running 애니메이션 실행
        }
        
        // 회전
        if (movement != Vector3.zero) // 키 입력값이 존재할 때
        {
            Quaternion toRotation = Quaternion.LookRotation(movement, Vector3.up); // 해당 방향을 바라봄
            transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, turnSpeed * Time.deltaTime); // 부드러운 회전
        }
    }
    
    // 경사로 판단 함수
    bool IsOnSlope()
    {
        Ray ray = new Ray(transform.position, Vector3.down);
        if (Physics.Raycast(ray, out slopeHit, groundCheckDistance, Ground))
        {
            var angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle != 0f && angle < maxSlopeAngle;
        }
        return false;
    }

    protected Vector3 SlopeDirection(Vector3 direction)
    {
        Vector3 slopeDirection = Vector3.ProjectOnPlane(direction, slopeHit.normal).normalized;
        return slopeDirection;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 boxSize = new Vector3(transform.lossyScale.x - 0.4f, 0.1f, transform.lossyScale.z - 0.4f);
        Gizmos.DrawWireCube(groundCheck.position, boxSize);
    }

    void GroundCheck()
    {
        Vector3 boxSize = new Vector3(transform.lossyScale.x - 0.4f, 0.1f, transform.lossyScale.z - 0.4f);
        
        if(Physics.CheckBox(groundCheck.position, boxSize, Quaternion.identity, Ground))
        {
            if (isJumping)
            {
                isGrounded = false;
            }
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }
    
    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.F) && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse); // Add an instant force impulse to the rigidbody, using its mass. From: Unity Script
            isJumping = true;
            anim.SetTrigger("Jumping");            
        }
    }

    
    void Block()
    {
        if (Input.GetMouseButton(1) && movement == Vector3.zero) // 정지 상태에서 오른쪽 마우스 버튼을 누를 때
        {
            anim.SetBool("Blocking", true);

            if (movement != Vector3.zero) // 움직임이 있을 때
            {
                anim.SetBool("Blocking", false);
                anim.SetBool("BlockingRun", true); // BlockingRun 애니메이션 활성화
            }
            else
            {
                anim.SetBool("BlockingRun", false); // 움직임이 없을 때 BlockingRun 비활성화
                anim.SetBool("Blocking", true);
            }
        }
        
        if (Input.GetMouseButtonUp(1)) // 오른쪽 마우스 버튼을 뗄 때
        {
            anim.SetBool("Blocking", false);
            anim.SetBool("BlockingRun", false); // 방어를 해제할 때 BlockingRun도 비활성화
        }
    }
    
    void Dodge()
    {
        if (isDodging)
        {
            rb.AddForce(transform.forward * 0.5f, ForceMode.Impulse);
        }
        if (Input.GetButtonDown("Dodge") && isGrounded && !isDodging)
        {
            isDodging = true;
            rb.drag = 10;
            anim.SetTrigger("Dodging");
            rb.AddForce(transform.forward * dodgeForce, ForceMode.Impulse);
            StartCoroutine(IsPlayerDodge());
        }
    }
    
    IEnumerator IsPlayerDodge()
    {
        yield return new WaitForSeconds(0.9f);
        isDodging = false;
    }
    
}