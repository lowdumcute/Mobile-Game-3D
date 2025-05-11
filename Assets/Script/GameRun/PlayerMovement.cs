using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement Instance { get; private set; }

    public float moveSpeed = 5f;
    public float jumpForce = 5f;
    public float gravityMultiplier = 2.0f;
    public float airSlideGravityMultiplier = 4.0f; // trọng lực tăng khi slide trên không
    public float slideSpeed = 5f;
    public Animator animator;

    private bool isMovingLane = false;
    private float laneMoveDuration = 0.2f;
    private float laneDistance = 1f;

    private CapsuleCollider capsuleCollider;
    private Rigidbody rb;
    private bool isGrounded = true;
    private bool isDead = false;
    private bool isSliding = false; // biến theo dõi trượt
    private Vector2 startTouchPosition;
    private Vector2 endTouchPosition;
    private float swipeThreshold = 50f;


    void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    void Start()
    {
        isDead = false;
        capsuleCollider = GetComponent<CapsuleCollider>();
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (isDead) return;

        transform.Translate(Vector3.right * moveSpeed * Time.deltaTime);

    #if UNITY_EDITOR || UNITY_STANDALONE
        // Chạy thử bằng chuột
        if (Input.GetMouseButtonDown(0))
            startTouchPosition = Input.mousePosition;
        else if (Input.GetMouseButtonUp(0))
        {
            endTouchPosition = Input.mousePosition;
            DetectSwipe();
        }
    #elif UNITY_ANDROID || UNITY_IOS
        // Trên thiết bị cảm ứng
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
                startTouchPosition = touch.position;
            else if (touch.phase == TouchPhase.Ended)
            {
                endTouchPosition = touch.position;
                DetectSwipe();
            }
        }
    #endif

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
        animator.Play("Jump");
    }

    public void Slide()
    {
        isSliding = true;
        animator.SetBool("Slide",true);
        capsuleCollider.radius = 0.4f;
        capsuleCollider.center = new Vector3(capsuleCollider.center.x, 0.42f, capsuleCollider.center.z);
        StartCoroutine(StopSlide());
    }

    public IEnumerator StopSlide()
    {
        yield return new WaitForSeconds(0.5f); // Thời gian trượt
        isSliding = false;
        animator.SetBool("Slide",false);
        capsuleCollider.radius = 0.66f;
        capsuleCollider.center = new Vector3(capsuleCollider.center.x, 0.6f, capsuleCollider.center.z);
    }

    void FixedUpdate()
    {
        if (isDead) return;

        float gravity = -9.81f * (isSliding && !isGrounded ? airSlideGravityMultiplier : gravityMultiplier);
        Vector3 gravityForce = new Vector3(0, gravity, 0);
        rb.AddForce(gravityForce, ForceMode.Acceleration);
    }

    private void OnCollisionStay(Collision collision) => isGrounded = true;
    private void OnCollisionExit(Collision collision) => isGrounded = false;

    public IEnumerator Die()
    {
        AudioManager.Instance.PlayVFX("Hit");
        isDead = true;
        rb.linearVelocity = Vector3.zero;
        animator.Play("Lose");
        yield return new WaitForSeconds(2f);
        StartCoroutine(GameplayManager.Instance.GameOver());
    }

    public void ResetPlayer()
    {
        isDead = false;
        transform.position = Vector3.zero;
        rb.linearVelocity = Vector3.zero;
        animator.Play("Running");
    }

    public void MoveLeft()
    {
        if (isDead || isMovingLane) return;
        float currentZ = transform.position.z;
        if (currentZ >= 1f) return;
        Vector3 targetPos = new Vector3(transform.position.x, transform.position.y, currentZ + laneDistance);
        StartCoroutine(MoveToLane(targetPos));
    }

    public void MoveRight()
    {
        if (isDead || isMovingLane) return;
        float currentZ = transform.position.z;
        if (currentZ <= -1f) return;
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
    private void DetectSwipe()
    {
        Vector2 swipeDelta = endTouchPosition - startTouchPosition;

        if (swipeDelta.magnitude > swipeThreshold)
        {
            float x = swipeDelta.x;
            float y = swipeDelta.y;

            if (Mathf.Abs(x) > Mathf.Abs(y))
            {
                if (x > 0) MoveRight();
                else MoveLeft();
            }
            else
            {
                if (y > 0) Jump();
                else Slide();
            }
        }
    }

}
