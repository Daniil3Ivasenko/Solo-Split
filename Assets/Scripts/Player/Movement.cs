using System;
using UnityEngine;

public class Movement : MonoBehaviour
{
    PlayerInput input;
    private bool inputActive = true;

    [Header("isGrounded")]
    [SerializeField] private float JumpSphereRadius;
    [SerializeField] private LayerMask mask;
    [SerializeField] private float JumpSphereY;
    [SerializeField] private float JumpStrenght;
    bool isGrounded;
    [Header("Player")]
    [SerializeField] private float WalkSpeed = 10;
    [SerializeField] private float RunSpeed = 20;
    Rigidbody rb;
    [Header("Camera")]
    public Transform Camera;
    private float _currentAngle;
    [SerializeField] private float MaxAngle = 90;
    [SerializeField] private float MinAngle = -90;
    [Header("Inventory")]
    [SerializeField] private GameObject inventoryPanel;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        input = new PlayerInput();
        input.Player.Inventory.performed += context => OpenInventory();
        input.Player.Jump.performed += context => Jump();
        LockCursor(true);
    }

    private void FixedUpdate()
    {
        CameraMovement();
        Move();
    }

    void CameraMovement()
    {
        Vector2 direction = input.Player.Look.ReadValue<Vector2>();
        _currentAngle -= direction.y;
        _currentAngle = Mathf.Clamp(_currentAngle, MinAngle, MaxAngle);
        Camera.localRotation = Quaternion.Euler(_currentAngle, 0, 0);
        transform.Rotate(0, direction.x, 0, Space.World);
    }

    void Move()
    {
        Vector2 direction = input.Player.Move.ReadValue<Vector2>();
        float RunForward = direction.y < 0 ? direction.y * WalkSpeed : direction.y * RunSpeed;
        Vector3 movement = ((transform.right * direction.x * (input.Player.Sprint.ReadValue<float>() > 0 && direction.y > 0 ? RunSpeed: WalkSpeed)) + 
            (transform.forward * (input.Player.Sprint.ReadValue<float>() > 0 ? RunForward : direction.y * WalkSpeed)));
        if (IsGrounded())
        {
            rb.linearVelocity = new Vector3(movement.x, movement.y, movement.z);
        }
    }
    void Jump()
    {
        if (IsGrounded())
        {
            rb.AddForce(transform.up * JumpStrenght, ForceMode.Impulse);
        }
    }
    
    private bool IsGrounded()
    {
        Vector3 SpherePosition = new Vector3(transform.position.x,transform.position.y + JumpSphereY,transform.position.z);
        bool grounded = Physics.CheckSphere(SpherePosition, JumpSphereRadius, mask);
        return grounded;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y + JumpSphereY, transform.position.z), JumpSphereRadius);
    }

    void LockCursor(bool a)
    {
        Cursor.lockState = a ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !a;
    }

    void OpenInventory()
    {
        LockCursor(inventoryPanel.activeSelf);
        inventoryPanel.SetActive(!inventoryPanel.activeSelf);
        if (inputActive) input.Player.Look.Disable();
        else
        {
            input.Player.Look.Enable();
            ActionManager.inventoryClosed?.Invoke();
        }
        inputActive = !inputActive;
    }
    private void OnEnable()
    {
        input.Enable();
        inputActive = true;
    }
    private void OnDisable()
    {
        input.Disable();
        inputActive = false;
    }
}
