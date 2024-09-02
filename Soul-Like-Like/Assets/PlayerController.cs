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
    public LayerMask ground;
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
        GroundCheck();
        Move();
        Jump();
        
        Debug.DrawRay(transform.position, Vector3.down * groundCheckDistance, Color.red);
        Debug.Log(isGrounded);
    }

    void Move()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        // 벡터의 정규화를 통해서 모든 방향의 이동속도를 동일하게 만든다.
        movement = new Vector3(horizontal, 0, vertical).normalized;

        // 이동
        transform.position += movement * moveSpeed * Time.deltaTime;
        
        // 애니메이션
        anim.SetBool("Running", movement != Vector3.zero);
        
        // 회전
        if (movement != Vector3.zero) // 키 입력값이 존재할 때
        {
            Quaternion toRotation = Quaternion.LookRotation(movement, Vector3.up); // 해당 방향을 바라봄
            transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, turnSpeed * Time.deltaTime); // 부드러운 회전
        }
    }

    void Jump()
    {
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            Debug.Log("jump!~");
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse); // Add an instant force impulse to the rigidbody, using its mass.
            anim.SetBool("Jumping", isGrounded);
        }
    }
    
    void GroundCheck()
    {
        
        // 플레이어의 위치에서, 아래방향으로, groundDistance 만큼 ray를 쏴서, ground 레이어가 있는지 검사
        if (Physics.Raycast(transform.position, Vector3.down, groundCheckDistance, ground))
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }
}
