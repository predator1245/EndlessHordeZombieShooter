using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MovementComponent : MonoBehaviour
{
    [SerializeField]
    float walkSpeed = 5;
    [SerializeField]
    float runSpeed = 10;
    [SerializeField]
    float jumpForce = 5;

    PlayerController playerController;
    Rigidbody rigidbody;
    Animator playerAnimator;
    public GameObject followTransform;

    Vector2 inputVector = Vector2.zero;
    Vector3 moveDirection = Vector3.zero;
    Vector2 lookInput = Vector2.zero;

    public float aimSensitivity;

    public readonly int movementXHash = Animator.StringToHash("MovementX");
    public readonly int movementYHash = Animator.StringToHash("MovementY");
    public readonly int isJumpingHash = Animator.StringToHash("IsJumping");
    public readonly int isRunningHash = Animator.StringToHash("IsRunning");

    private void Awake()
    {
        playerAnimator = GetComponent<Animator>();
        playerController = GetComponent<PlayerController>();
        rigidbody = GetComponent<Rigidbody>();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        followTransform.transform.rotation *= Quaternion.AngleAxis(lookInput.x * aimSensitivity, Vector3.up);
        followTransform.transform.rotation *= Quaternion.AngleAxis(lookInput.y * aimSensitivity, Vector3.left);

        var angles = followTransform.transform.localEulerAngles;
        angles.z = 0;

        var angle = followTransform.transform.localEulerAngles.x;


        if (angle > 180 && angle < 300)
        {
            angles.x = 300;
        }
        else if(angle < 180 && angle > 70)
        {
            angles.x = 70;
        }

        followTransform.transform.localEulerAngles = angles;
        transform.rotation = Quaternion.Euler(0, followTransform.transform.rotation.eulerAngles.y, 0);
        followTransform.transform.localEulerAngles = new Vector3(angles.x, 0, 0);

        if (playerController.isJumping) return;
        if (!(inputVector.magnitude > 0)) moveDirection = Vector3.zero;

        moveDirection = transform.forward * inputVector.y + transform.right * inputVector.x;
        float currentSpeed = playerController.isRunning ? runSpeed : walkSpeed;

        Vector3 movementDirection = moveDirection * (currentSpeed * Time.deltaTime);
        transform.position += movementDirection;
    }

    public void OnMovement(InputValue value)
    {
        inputVector = value.Get<Vector2>();
        playerAnimator.SetFloat(movementXHash, inputVector.x);
        playerAnimator.SetFloat(movementYHash, inputVector.y);
    }

    public void OnRun(InputValue value)
    {
        playerController.isRunning = value.isPressed;
        playerAnimator.SetBool(isRunningHash, playerController.isRunning);
    }

    public void OnJump(InputValue value)
    {
        playerController.isJumping = value.isPressed;               
        rigidbody.AddForce((transform.up + moveDirection) * jumpForce, ForceMode.Impulse);
        playerAnimator.SetBool(isJumpingHash, playerController.isJumping);
    }

    public void OnAim(InputValue value)
    {
        playerController.isAiming = value.isPressed;
    }
    public void OnLook(InputValue value)
    {
        lookInput = value.Get<Vector2>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Ground") && !playerController.isJumping) return;

        playerController.isJumping = false;
        playerAnimator.SetBool(isJumpingHash, false);
    }
}