using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.AI;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    private PlayerInputActions inputActions;
    private CharacterController controller;
    private Vector2 inputVector;
    private Vector3 moveDirection;
    public float moveSpeed = 5f;
    public float gravity = -9.81f;

    private float velocityY;

    void Awake()
    {
        inputActions = new PlayerInputActions();
        controller = GetComponent<CharacterController>();
    }

    void OnEnable()
    {
        inputActions.Player.Enable();
        inputActions.Player.Move.performed += ctx => inputVector = ctx.ReadValue<Vector2>();
        inputActions.Player.Move.canceled += ctx => inputVector = Vector2.zero;
    }

    void OnDisable()
    {
        inputActions.Player.Disable();
    }

    void Update()
    {
        Vector3 move = new Vector3(inputVector.x, 0, inputVector.y);
        moveDirection = move.normalized * moveSpeed;

        // Gravity
        if (controller.isGrounded)
            velocityY = -1f;
        else
            velocityY += gravity * Time.deltaTime;

        moveDirection.y = velocityY;

        controller.Move(moveDirection * Time.deltaTime);

        // Xoay theo hướng di chuyển
        if (move != Vector3.zero)
            transform.forward = move;
    }
}
