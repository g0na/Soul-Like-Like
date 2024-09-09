using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float turnSpeed = 5f;

    private bool isGrounded;
    public float groundCheckDistance = 0.1f;
    public float jumpForce = 5f;
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
        
        Move();
        Jump();
        Block();
        
    }

    void Move()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        // 벡터의 정규화를 통해서 모든 방향의 이동속도를 동일하게 만든다.
        // movement = new Vector3(horizontal, 0, vertical).normalized;
        movementVertical = Vector3.Normalize(new Vector3(this.transform.position.x - Camera.transform.position.x, 0, this.transform.position.z - Camera.transform.position.z)) * vertical;
        movementHorizontal = Vector3.Normalize(new Vector3(Camera.transform.position.y - this.transform.position.y, 0, Camera.transform.position.x - this.transform.position.x)) * horizontal;

        Debug.Log(movement);

        movement = movementVertical + movementHorizontal;

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
    
    void GroundCheck()
    {
        // 플레이어의 위치에서, 아래방향으로, groundCheckDistance 만큼 ray를 쏴서, Ground 레이어가 있는지 검사
        if (Physics.Raycast(transform.position, Vector3.down, groundCheckDistance, Ground))
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }
    
    void Jump()
    {
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse); // Add an instant force impulse to the rigidbody, using its mass. From: Unity Script
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
    
}