using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class movement : MonoBehaviour
{
    [SerializeField] private float walkForce = 5f;
    [SerializeField] private float maxSpeed = 5f;
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float runSpeed = 10f;
    [SerializeField] private float crouchSpeed = 2f;
    private Vector2 moveInput;
    private Rigidbody rb;
    private bool isGrounded = true;
    private InputAction moveAction;
    private InputAction crouchAction;
    private InputAction runAction;
    private InputAction jumpAction;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        moveAction = InputSystem.actions.FindAction("Move");
        crouchAction = InputSystem.actions.FindAction("Crouch");
        runAction = InputSystem.actions.FindAction("Sprint");
        jumpAction = InputSystem.actions.FindAction("Jump");
        jumpAction.performed += ctx => Jump();
        crouchAction.performed += ctx => Crouch();
        crouchAction.canceled += ctx => UnCrouch();
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = Physics.OverlapSphere(transform.position - new Vector3(0,0.505f) * transform.localScale.y, 0.5f, LayerMask.GetMask("Ground")).Length > 0;
        if(rb.linearVelocity.magnitude > 0.05)
            transform.LookAt(transform.position + rb.linearVelocity - new Vector3(0,rb.linearVelocity.y,0));
    }

    private void FixedUpdate()
    {
        moveInput = moveAction.ReadValue<Vector2>();
        if (runAction.IsPressed())
        {
            maxSpeed = runSpeed;
        }
        else if (crouchAction.IsPressed())
        {
            maxSpeed = crouchSpeed;
        }
        else
        {
            maxSpeed = walkSpeed;
        }
        Vector3 movement = (Vector3.ProjectOnPlane(Camera.main.transform.forward, Vector3.up).normalized*moveInput.y+ Camera.main.transform.right*moveInput.x) * walkForce;
        rb.AddForce(movement * (1 - Mathf.Clamp01(rb.linearVelocity.magnitude / maxSpeed) * Vector3.Dot(rb.linearVelocity.normalized, movement.normalized)), ForceMode.Force);
    }

    private void Jump()
    {
        if (isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
        }
    }

    private void Crouch()
    {
        transform.localScale = new Vector3(1, 0.5f, 1);
        transform.position += new Vector3(0, -0.25f, 0);
    }

    private void UnCrouch()
    {
        transform.localScale = new Vector3(1, 1, 1);
        transform.position += new Vector3(0, 0.25f, 0);
    }
}
