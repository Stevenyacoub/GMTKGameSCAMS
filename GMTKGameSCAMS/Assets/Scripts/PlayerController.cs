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
    private float magnitude;
    private Vector3 moveDirection;
    private Vector3 moveForce;

    [SerializeField] private float jumpSpeed;

    PlayerInput inputActions;
    InputAction move;

    private WallState currentWallState = WallState.Wall;

    [SerializeField] private float maxJumpHeight = 1.0f;
    [SerializeField] private float maxJumpDuration = 0.5f;
    private float jumpInit;
    private bool isJumpPressed = false;
    private bool isJumping = false;

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

        Debug.Log(move.ReadValue<Vector2>());
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
        if (currentWallState == WallState.Wall)
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
        Debug.Log(isJumpPressed);
        handleJump();
    }

    private void handleJump()
    {
        if (!isJumping && isJumpPressed)
        {
            isJumping = true;
        }
    }

    // Update is called once per frame
    void Update()
    {

        transform.Translate(-1 * moveDirection * magnitude * moveSpeed * Time.deltaTime, Space.World);

        if (moveDirection != Vector3.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(-1 * moveDirection, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
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
        Debug.Log(WallState.Roaming3D);
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
        Debug.Log(WallState.Wall);
    }
}
