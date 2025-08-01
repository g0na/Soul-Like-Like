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
    public bool isAlive;
    private bool isParrying = false;
    private float parryDuration = 1f; // 패링 지속 시간
    [SerializeField]
    public Transform groundCheck;

    private GameObject lastRaycastHitEnemy = null;
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
    private Enemy _enemy;
    private Boss _Boss;

    Rigidbody rb;
    [SerializeField]
    public Animator anim;
    Attack _attack;

    public GameObject Camera;
    public int maxHp;
    public int currentHp;

    [HideInInspector]
    public bool isBonFire;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        _attack = GetComponent<Attack>();
        isAlive = true;
    }

    // Update is called once per frame
    void Update()
    {
        GroundCheck();
        if (isAlive)
        {
            if (!isDodging)
            {
                Move();
            }
            MainAttack();
            Parry();
            Jump();
            Block();
            Dodge();
            Fall();
            OutofMap();
            ShootRaycast();
            Death();
            Interaction();
        }
    }

    private void Death()
    {
        if (currentHp <= 0)
        {
            isAlive = false;
            anim.SetTrigger("Death");
            UIManager.Instance.ShowDeadPanel();
            // Destroy(gameObject, 4f);
        }
    }

    void Move()
    {
        if (isAttacking || isParrying)
        {
            return;
        }

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        bool isOnSlope = IsOnSlope();

        movementVertical = Vector3.Normalize(new Vector3(this.transform.position.x - Camera.transform.position.x, 0, this.transform.position.z - Camera.transform.position.z)) * vertical;

        Vector3 arrangedTransformPosition = new Vector3(this.transform.position.x, 0, this.transform.position.z);
        Vector3 arrangedCameraPosition = new Vector3(Camera.transform.position.x, 0, Camera.transform.position.z);

        movementHorizontal = Vector3.Normalize(Vector3.Cross(arrangedCameraPosition - arrangedTransformPosition, this.transform.position - arrangedTransformPosition)) * horizontal;

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

        if (Physics.CheckBox(groundCheck.position, boxSize, Quaternion.identity, Ground))
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
        yield return new WaitForSeconds(0.25f);
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


    private void OnTriggerStay(Collider other)
    {
        // 패링 상태가 아니면 무시
        if (!isParrying) return;

        // Enemy 태그를 가진 오브젝트인지 확인
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();

            // Enemy가 null이 아니고 패링 가능 상태인지 확인
            if (enemy != null && enemy.isParriable)
            {
                // 패링 성공 처리
                enemy.GetCounterAttack();
                Debug.Log("패링 성공");
            }
        }
    }
    void ShootRaycast()
    {
        Ray ray = new Ray(transform.position + new Vector3(0f, 0.75f, 0f), transform.forward);
        RaycastHit hit;

        Debug.DrawRay(ray.origin, ray.direction * 2f, Color.red);

        lastRaycastHitEnemy = null;

        if (Physics.Raycast(ray, out hit, 2f))
        {
            if (hit.transform.CompareTag("Enemy"))
            {
                lastRaycastHitEnemy = hit.transform.gameObject;
            }
        }
    }

    //public void Get_Damage(int iDamage, GameObject damager)
    //{
    //    if (damager.gameObject.CompareTag("Enemy"))
    //    {
    //        _enemy = damager.gameObject.GetComponent<Enemy>();

    //        if (_enemy != null)
    //        {
    //            // 현재 충돌한 Enemy가 Raycast로 감지된 Enemy와 같은지 확인
    //            if (lastRaycastHitEnemy == damager.gameObject)
    //            {
    //                // 정면 충돌
    //                anim.SetTrigger("Hit_Front");
    //                currentHp -= iDamage;
    //                StartCoroutine(After_Get_Damaged());
    //            }
    //            else
    //            {
    //                // 후면 충돌
    //                anim.SetTrigger("Hit_Back");
    //                currentHp -= iDamage;
    //                StartCoroutine(After_Get_Damaged());
    //            }
    //        }
    //    }
    //    UIManager.Instance.ChangeHealth();
    //}

    // 애니메이터의 상태를 기준으로 isBlocking을 판단
    public bool isBlocking
    {
        get
        {
            return anim.GetBool("Blocking") || anim.GetBool("BlockingRun");
        }
    }

    public void Get_Damage(int iDamage, GameObject damager)
    {
        if (damager.gameObject.CompareTag("Enemy"))
        {
            _enemy = damager.gameObject.GetComponent<Enemy>();

            if (_enemy != null)
            {
                // 현재 충돌한 Enemy가 Raycast로 감지된 Enemy와 같은지 확인
                // 동시에 방어 중인지도 체크
                if (isBlocking && IsFacing(damager.transform))
                {
                    //iDamage = Mathf.Max(0, iDamage - 10);
                    return;                    
                }
                else
                {
                    // 정면/후면 충돌에 따라 애니메이션 실행
                    anim.SetTrigger(lastRaycastHitEnemy == damager ? "Hit_Front" : "Hit_Back");
                }

                currentHp -= iDamage;
                StartCoroutine(After_Get_Damaged());
            }
        }

        if (damager.gameObject.CompareTag("Boss"))
        {
            _Boss = damager.gameObject.GetComponent<Boss>();

            if (_Boss != null)
            {
                // 현재 충돌한 Enemy가 Raycast로 감지된 Enemy와 같은지 확인
                // 동시에 방어 중인지도 체크
                if (isBlocking && IsFacing(damager.transform))
                {
                    //iDamage = Mathf.Max(0, iDamage - 10);
                    return;                    
                }
                else
                {
                    // 정면/후면 충돌에 따라 애니메이션 실행
                    anim.SetTrigger(lastRaycastHitEnemy == damager ? "Hit_Front" : "Hit_Back");
                }

                currentHp -= iDamage;
                StartCoroutine(After_Get_Damaged());
            }
        }
        UIManager.Instance.ChangeHealth();
    }

    // 정면 방어만 막을 수 있도록 하는 함수, 방어 중이더라도 뒤에서 맞으면 안되니깐
    private bool IsFacing(Transform enemy)
    {
        Vector3 directionToEnemy = (enemy.position - transform.position).normalized;
        float dot = Vector3.Dot(transform.forward, directionToEnemy);
        return dot > 0.7f; // 시야 정면 약180도 정도
    }


    IEnumerator After_Get_Damaged()
    {
        gameObject.tag = "Invincible";
        yield return new WaitForSeconds(1.0f);
        gameObject.tag = "Player";
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

    void Parry()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (isParrying) return;
            
            anim.SetTrigger("Parry");
            isParrying = true;
            
            StartCoroutine(Parry_End());
        }
    }
    
    IEnumerator Parry_End()
    {
        yield return new WaitForSeconds(parryDuration);
        isParrying = false;
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

    void Interaction()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (isBonFire)
            {
                Rest();
                Debug.Log("Rest");
            }
        }
    }

    void Rest()
    {        
        anim.SetBool("Sitting",true);
        currentHp = maxHp;
        UIManager.Instance.ChangeHealth();
        GameManager.Instance.Rest();

        // UIManager.Instance.


    }

    public void RestEnd()
    {
        //
    }
}