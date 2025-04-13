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
    public bool isAttacking;
    [SerializeField] 
    public Transform groundCheck;
    

    private RaycastHit slopeHit;
    public float groundCheckDistance;
    public Transform raycastOrigin;
    public float maxSlopeAngle;
    public float jumpForce;
    public LayerMask Ground;

    private Vector3 movement;
    private Vector3 movementVertical;
    private Vector3 movementHorizontal;

    public Sword sword;
    
    Rigidbody rb;
    Animator anim;
    Attack _attack;

    public GameObject Camera;
    public int maxHp;
    public int currentHp;
    
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        _attack = GetComponent<Attack>();
    }

    // Update is called once per frame
    void Update()
    {
        GroundCheck();
        if (!isDodging)
        {
            Move();
        }
        MainAttack();
        Jump();
        Block();
        Dodge();
        Fall();
        OutofMap();
    }



    void Move()
    {
        if (isAttacking)
        {
            return;
        }
        
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        bool isOnSlope = IsOnSlope();

        movementVertical = Vector3.Normalize(new Vector3(this.transform.position.x - Camera.transform.position.x, 0, this.transform.position.z - Camera.transform.position.z)) * vertical;

        Vector3 arrangedTransformPosition = new Vector3(this.transform.position.x, 0, this.transform.position.z);
        Vector3 arrangedCameraPosition = new Vector3(Camera.transform.position.x, 0, Camera.transform.position.z);

        movementHorizontal = Vector3.Normalize(Vector3.Cross(arrangedCameraPosition - arrangedTransformPosition, this.transform.position - arrangedTransformPosition )) * horizontal;

        if (CheckNextFrameAngle(moveSpeed) < maxSlopeAngle)
        {
            movement = movementVertical + movementHorizontal;
        }
        else
        {
            movement = Vector3.zero;
        }

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
            // rb.useGravity = false;
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
            Quaternion toRotation = Quaternion.LookRotation(movement, Vector3.up); // 이동 방향을 바라보도록 설정
            Vector3 eulerRotation = toRotation.eulerAngles;
            eulerRotation.x = 0f; // X축 회전 제거
            eulerRotation.z = 0f; // Z축 회전 제거
            toRotation = Quaternion.Euler(eulerRotation); // 수정된 회전 값 적용
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
            isGrounded = true;
            anim.SetBool("Falling", false);
        }
        else
        {
            isGrounded = false;
        }
    }

    private float CheckNextFrameAngle(float moveSpeed)
    {
        // 캐릭터의 다음 프레임 위치
        var nextFramePosition = raycastOrigin.position + movement * moveSpeed * Time.deltaTime;

        if (Physics.Raycast(nextFramePosition, Vector3.down, out RaycastHit hitInfo, groundCheckDistance, Ground))
        {
            return Vector3.Angle(Vector3.up, hitInfo.normal);
        }

        return 0f;
    }
    
    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.F) && isGrounded && !isDodging && !isAttacking)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse); // Add an instant force impulse to the rigidbody, using its mass. From: Unity Script
            isJumping = true;
            isGrounded = false;
            anim.SetTrigger("Jumping");            
        }
    }

    void Fall()
    {
        if (!isGrounded && !isJumping)
        {
            // anim.SetTrigger("Falling");
            StartCoroutine(DelayedFallAnimation());
        }
    }
    
    IEnumerator DelayedFallAnimation()
    {
        yield return new WaitForSeconds(0.25f); // 1초 대기
        if (!isGrounded) // 여전히 지면에 닿아있지 않은 경우에만 실행
        {
            anim.SetTrigger("Falling");
        }
    }

    void JumpFall()
    {
        anim.SetTrigger("Falling");
    }
    
    
    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            isJumping = false;
            isGrounded = true;
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
        if (Input.GetButtonDown("Dodge") && isGrounded && !isDodging)
        {
            isDodging = true;
            anim.SetTrigger("Dodging");

            // 경사로 방향을 고려한 구르기 방향 설정
            Vector3 dodgeDirection = IsOnSlope() ? SlopeDirection(transform.forward) : transform.forward;
            rb.velocity = Vector3.zero;
            rb.AddForce(dodgeDirection * dodgeForce, ForceMode.VelocityChange);

            StartCoroutine(IsPlayerDodge());
        }
    }

    IEnumerator IsPlayerDodge()
    {
        float dodgeTime = 0.75f;
        float groundStayForce = 0.8f; // 경사로 쪽 힘

        for (float t = 0; t < dodgeTime; t += Time.deltaTime)
        {
            if (IsOnSlope())
            {
                // 경사로 위에서 지면을 유지하도록 추가 힘 적용
                rb.AddForce(-slopeHit.normal * groundStayForce, ForceMode.Acceleration);
            }

            yield return null;
        }
        
        rb.drag = 10f;
        yield return new WaitForSeconds(0.1f);
        rb.velocity = Vector3.zero;
        rb.drag = 0f;
        
        isDodging = false;
    }

    void MainAttack()
    {
        if (sword == null)
            return;
        
        if (Input.GetMouseButtonDown(0) && isGrounded && !isDodging)
        {
            sword.Use();
            anim.SetTrigger("Attack");
            _attack.AttackCount = 0;
        }
    }
    
    void StartAttack()
    {
        isAttacking = true;
        moveSpeed = 0f;
        turnSpeed = 0f;

        sword.attackArea.enabled = true;
    }

    void StopAttack()
    {
        isAttacking = false;
        moveSpeed = 5f;
        turnSpeed = 50f;
        
        sword.attackArea.enabled = false;
    }

    private void OutofMap()
    {
        if (this.transform.position.y < -12f)
        {
            this.transform.position = new Vector3(0, 1, 0);
            currentHp = 0;
            UIManager.Instance.ChangeHealth();
        }
    }
}