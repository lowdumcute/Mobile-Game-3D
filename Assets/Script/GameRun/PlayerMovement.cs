using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement Instance { get; private set; }

    public float moveSpeed = 5f;  // Tốc độ di chuyển
    public float jumpForce = 5f;  // Lực nhảy
    public float gravityMultiplier = 2.0f;  // Hệ số trọng lực
    public float slideSpeed = 5f;  // Tốc độ trượt
    public Animator animator;

    private Rigidbody rb;
    private bool isGrounded = true;
    private bool isDead = false;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (isDead) return;

        transform.Translate(Vector3.right * moveSpeed * Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Jump();
        }
    }

    public void Jump()
    {
        if (!isGrounded) return;
        AudioManager.Instance.PlayVFX("Jump");
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        animator.SetTrigger("Jump");
    }

    public void Slide()
    {
        animator.SetBool("Slide", true);
    }

    public void StopSlide()
    {
        animator.SetBool("Slide", false);
    }

    void FixedUpdate()
    {
        if (isDead) return;

        Vector3 gravityForce = new Vector3(0, -9.81f * gravityMultiplier, 0);
        rb.AddForce(gravityForce, ForceMode.Acceleration);
    }

    private void OnCollisionStay(Collision collision)
    {
        isGrounded = true;
    }

    private void OnCollisionExit(Collision collision)
    {
        isGrounded = false;
    }


    // Gọi khi nhân vật chết
    public IEnumerator Die()
    {  
        AudioManager.Instance.PlayVFX("Hit"); 
        isDead = true;
        rb.linearVelocity = Vector3.zero;
        animator.SetTrigger("Lose");
        yield return new WaitForSeconds(2f); // Thời gian hoạt ảnh chết
        GameplayManager.Instance.GameOver();
    }
}
