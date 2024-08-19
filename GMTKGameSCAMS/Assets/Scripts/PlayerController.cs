using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public enum WallState
    {
        Wall,
        Roaming3D
    }

    [SerializeField] private float moveSpeed;
    [SerializeField] private float maxSpeed;
    [SerializeField] public float rotationSpeed;
    private bool freezeRotation;
    private float magnitude;
    private Vector3 moveDirection;
    private Vector3 moveForce;

    [SerializeField] private float jumpSpeed;

    PlayerInput inputActions;
    InputAction move;

    private WallState currentWallState = WallState.Wall;
    private GameObject collidedWall;

    [SerializeField] private float maxJumpHeight = 1.0f;
    [SerializeField] private float maxJumpDuration = 0.5f;
    private float jumpInit;
    private bool isJumpPressed = false;
    private bool isGrounded = true;
    
    private bool isHoldPressed = false;
    private GameObject collidingObject;
    private GameObject currentHeldObject;

    [SerializeField] private Animator animator;
    

    [SerializeField] private Rigidbody rb;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        inputActions = new PlayerInput();

        inputActions.Grounded.Move.started += onMovementInput;
        inputActions.Grounded.Move.canceled += onMovementInput;
        inputActions.Grounded.Move.performed += onMovementInput;
        
        inputActions.Grounded.Jump.started += OnJump;
        inputActions.Grounded.Jump.canceled += OnJump;

        inputActions.Grounded.Enter.started += OnEnterWall;
        inputActions.Grounded.Enter.canceled -= OnEnterWall;
        
        inputActions.Grounded.Exit.started += OnExitWall;
        inputActions.Grounded.Exit.canceled -= OnExitWall;
        
        inputActions.Grounded.Hold.started += OnHoldObject;
        inputActions.Grounded.Hold.canceled -= OnHoldObject;
        
        // Player should begin on the wall
        OnEnterWallState();
    }


    private void onMovementInput(InputAction.CallbackContext context)
    {
        if (currentWallState == WallState.Roaming3D)
        {
            moveDirection = new Vector3(move.ReadValue<Vector2>().x, 0, move.ReadValue<Vector2>().y);
            moveDirection.Normalize();
            magnitude = moveDirection.magnitude;
            magnitude = Mathf.Clamp01(magnitude);
        }else if (currentWallState == WallState.Wall)
        {
            moveDirection = new Vector3(move.ReadValue<Vector2>().x, 0, 0);
            moveDirection.Normalize();
            magnitude = moveDirection.magnitude;
            magnitude = Mathf.Clamp01(magnitude);
        }

    }

    private void OnEnable()
    {
        move = inputActions.Grounded.Move;
        inputActions.Enable();
    }

    private void OnDisable()
    {
        move = inputActions.Grounded.Move;
        inputActions.Disable();
    }
    private void OnEnterWall(InputAction.CallbackContext context)
    {
        if (currentWallState == WallState.Wall || collidedWall == null)
        {
            return;
        }
        else if (currentWallState == WallState.Roaming3D)
        {
            SetSizeState(WallState.Wall);
        }
    }
    private void OnExitWall(InputAction.CallbackContext context)
    {
        if (currentWallState == WallState.Wall)
        {
            SetSizeState(WallState.Roaming3D);
        }
        else if (currentWallState == WallState.Roaming3D)
        {
            return;
        }
    }

    private void OnJump(InputAction.CallbackContext context)
    {
        isJumpPressed = context.ReadValueAsButton();
        handleJump();
    }

    private void handleJump()
    {
        Debug.Log("handle jump called. isGrounded is " + isGrounded);
        if (isGrounded && isJumpPressed)
        {
            isGrounded = false;
            Debug.Log(Vector3.up * jumpSpeed);
            rb.AddForce(Vector3.up * jumpSpeed, ForceMode.Impulse);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("ShadowableObject") || collision.gameObject.CompareTag("Shadow"))
        {
            isGrounded = true;
        }
    }

    private void OnHoldObject(InputAction.CallbackContext context)
    {
        isHoldPressed = !isHoldPressed;
        HandleHold();
    }

    private void HandleHold()
    {
        if (isHoldPressed && currentHeldObject == null && collidingObject != null)
        {
            currentHeldObject = collidingObject;
            currentHeldObject.transform.parent = transform;
            freezeRotation = true;
            // Debug.Log("isHoldPressed: " + isHoldPressed + ", collidingObjectParent: " + collidingObject.transform.parent.name);
        }
        else if (isHoldPressed == false && currentHeldObject != null)
        {
            currentHeldObject.transform.parent = null;
            currentHeldObject = null;
            freezeRotation = false;
            // Debug.Log("isHoldPressed: " + isHoldPressed + ", collidingObjectParent: " + collidingObject.transform.parent.name);
        }
    }

    // Update is called once per frame
    void Update()
    {

        transform.Translate(-1 * moveDirection * magnitude * moveSpeed * Time.deltaTime, Space.World);

        if (moveDirection != Vector3.zero)
        {
            if (freezeRotation == false)
            {
                Quaternion toRotation = Quaternion.LookRotation(-1 * moveDirection, Vector3.up);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
            }
            
            // Align player along the wall. Must align often because when the player turns around, it's collider
            // pushes it away from the wall.
            if (currentWallState == WallState.Wall && collidedWall != null)
            {
                float wallZPos = collidedWall.transform.position.z;
                transform.position = new Vector3(transform.position.x,
                    transform.position.y,
                    wallZPos);
            }
            
            animator.SetBool("IsMoving", true);
        }
        else
        {
            animator.SetBool("IsMoving", false);
        }
    }

    protected void SetSizeState(WallState argState)
    {
        if (argState == currentWallState)
        {
            return;
        }

        //WallState prevState = currentWallState;
        currentWallState = argState;

        /*switch (prevState)
        {
            case WallState.Wall:
                OnExitWallState();
                break;
            case WallState.Roaming3D:
                OnExitRoaming3DState();
                break;
        }*/

        switch (currentWallState)
        {
            case WallState.Wall:
                OnEnterWallState();
                break;
            case WallState.Roaming3D:
                OnEnterRoaming3DState();
                break;
        }
    }

    private void OnEnterRoaming3DState()
    {
        transform.localPosition += new Vector3(0.0f, 0.0f, 1.0f);
        transform.localScale = Vector3.one;
    }

    /*private void OnExitRoaming3DState()
    {
        transform.localScale = new Vector3(0.1f, 1.0f, 1.0f);
        Debug.Log(WallState.Wall);
    }

    private void OnExitWallState()
    {
        transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        Debug.Log(WallState.Roaming3D);
    }*/

    private void OnEnterWallState()
    {
        transform.localScale = new Vector3(0.01f, 1f, 1f);
    }
    
    void OnTriggerEnter(Collider other)
    {
        // Check if the other collider has the specific tag
        if (other.CompareTag("ShadowableObject"))
        {
            collidingObject = other.gameObject;
        }
        else if (other.CompareTag("Wall"))
        {
            collidedWall = other.gameObject;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("ShadowableObject"))
        {
            collidingObject = null;
        }
        else if (other.CompareTag("Wall"))
        {
            collidedWall = null;
        }
    }
}
