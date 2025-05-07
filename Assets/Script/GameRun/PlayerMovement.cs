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
    private bool isMovingLane = false;
    private float laneMoveDuration = 0.2f; // Thời gian chuyển làn
    private float laneDistance = 1f; // Khoảng cách mỗi làn (z = ±1)

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
        isDead = false;
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
        StartCoroutine(GameplayManager.Instance.GameOver());
    }
    public void ResetPlayer()
    {
        isDead = false;
        transform.position = Vector3.zero;
        rb.linearVelocity = Vector3.zero;
        animator.Play("Running"); // Nếu có animation mặc định
    }
    public void MoveLeft()
    {
        if (isDead || isMovingLane) return;

        float currentZ = transform.position.z;
        if (currentZ >= 1f) return;  // Giới hạn không di chuyển nếu vượt quá z = 1

        Vector3 targetPos = new Vector3(transform.position.x, transform.position.y, currentZ + laneDistance);
        StartCoroutine(MoveToLane(targetPos));
    }

    public void MoveRight()
    {
        if (isDead || isMovingLane) return;

        float currentZ = transform.position.z;
        if (currentZ <= -1f) return;  // Giới hạn không di chuyển nếu nhỏ hơn z = -1

        Vector3 targetPos = new Vector3(transform.position.x, transform.position.y, currentZ - laneDistance);
        StartCoroutine(MoveToLane(targetPos));
    }


    private IEnumerator MoveToLane(Vector3 targetPosition)
    {
        isMovingLane = true;
        float startZ = transform.position.z;
        float targetZ = targetPosition.z;
        float elapsed = 0f;
        while (elapsed < laneMoveDuration)
        {
            float currentZ = Mathf.Lerp(startZ, targetZ, elapsed / laneMoveDuration);
            transform.position = new Vector3(transform.position.x, transform.position.y, currentZ);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = new Vector3(transform.position.x, transform.position.y, targetZ);
        isMovingLane = false;
    }


}
