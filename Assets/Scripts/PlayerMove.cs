using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [Header("Move")]
    public float walkSpeed = 5f;
    public float runSpeed = 8f;
    public float jumpForce = 6f;

    [Header("Ground")]
    public Transform groundCheck;
    public LayerMask groundLayer;
    public float groundRadius = 0.25f;

    Rigidbody rb;
    Animator animator;
    PlayerStamina stamina;

    float h, v;
    bool isGrounded;
    bool isSprint;
    bool isCrounch;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
        stamina = GetComponent<PlayerStamina>();

        animator.applyRootMotion = false; // ❗ BẮT BUỘC
    }

    void Update()
    {
        // ===== INPUT =====
        h = Input.GetAxis("Horizontal");
        v = Input.GetAxis("Vertical");

        float speedValue = new Vector2(h, v).magnitude;
        animator.SetFloat("Speed", speedValue);

        // ===== SPRINT =====
        isSprint = Input.GetKey(KeyCode.LeftShift) && speedValue > 0.1f;
        animator.SetBool("isSprint", isSprint);

        // ===== CROUCH (TOGGLE) =====
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            isCrounch = !isCrounch;
            animator.SetBool("isCrounch", isCrounch);
        }

        // ===== GROUND CHECK =====
        isGrounded = Physics.CheckSphere(
            groundCheck.position,
            groundRadius,
            groundLayer
        );

        // ===== JUMP =====
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && !isCrounch)
        {
            Debug.Log("JUMP TRIGGERED");
            animator.SetTrigger("Jump");

            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    void FixedUpdate()
    {
        float speed = isSprint ? runSpeed : walkSpeed;

        Vector3 move =
            transform.forward * v +
            transform.right * h;

        rb.MovePosition(
            rb.position + move * speed * Time.fixedDeltaTime
        );
    }
}
