using System;
using UnityEngine;
using UnityEngine.UI;

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
    [SerializeField] public float stamina { get { return _stamina; } set { _stamina = Mathf.Clamp(value, 0f, 100f); slider.value = _stamina / 100; if (_stamina > 30) canRun = true;
            if (_stamina < 0.1) canRun = false;} }
    Rigidbody rb;
    public float _stamina;
    public bool canRun = true;
    public Slider slider;
    [Header("Camera")]
    public Transform Camera;
    private float _currentAngle;
    [SerializeField] private float MaxAngle = 90;
    [SerializeField] private float MinAngle = -90;
    [Header("Inventory")]
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private Inventory inventory;
    private float speedModifier;
    private float staminaModifier;

    void Awake()
    {
        ActionManager.ItemChanged += RecalculateOverWeightModifier;
        rb = GetComponent<Rigidbody>();
        input = new PlayerInput();
        input.Player.Inventory.performed += context => OpenInventory();
        input.Player.Jump.performed += context => Jump();
        input.Player.Attack.performed += context => UseItem();
        LockCursor(true);
        RecalculateOverWeightModifier();
    }

    private void FixedUpdate()
    {
        CameraMovement();
        Move();
        Debug.Log(inventory.overWeightCoefficient);
    }
    private void Update()
    {
        ChangeSelectedItem();
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
        Vector3 movement = ((transform.right * direction.x * (input.Player.Sprint.ReadValue<float>() > 0 && direction.y > 0 && canRun ? RunSpeed: WalkSpeed)) + 
            transform.forward * (input.Player.Sprint.ReadValue<float>() > 0 && canRun ? RunForward : direction.y * WalkSpeed)) * speedModifier;
        if (IsGrounded())
        {
            rb.linearVelocity = new Vector3(movement.x, movement.y, movement.z);
            if (direction == new Vector2(0, 0) || movement == ((transform.right * direction.x * WalkSpeed) +
                transform.forward * direction.y * WalkSpeed) * speedModifier)
                stamina += 0.2f * staminaModifier;
            else
                stamina -= 0.1f / staminaModifier;
        }
    }
    void Jump()
    {
        if (IsGrounded() && canRun)
        {
            rb.AddForce(transform.up * JumpStrenght * staminaModifier, ForceMode.Impulse);
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
        if (inputActive)
        {
            input.Player.Look.Disable();
            input.Player.Attack.Disable();
        }
        else
        {
            input.Player.Attack.Enable();
            input.Player.Look.Enable();
            ActionManager.inventoryClosed?.Invoke();
        }
        inputActive = !inputActive;
    }

    void UseItem()
    {
        inventory.inventory[inventory.selectedSlot].item?.Use();
    }
    void ChangeSelectedItem()
    {
        for (int i = 0; i < 10; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                inventory.selectedSlot = i;
            }
        }
    }

    void RecalculateOverWeightModifier()
    {
        if(inventory.overWeightCoefficient == 0)
        {
            speedModifier = 1;
            staminaModifier = 1;
        }
        else if (inventory.overWeightCoefficient == 1)
        {
            speedModifier = 0.7f;
            staminaModifier = 0.7f;
        }
        else if (inventory.overWeightCoefficient == 2)
        {
            speedModifier = 0.4f;
            staminaModifier = 0.7f;
        }
        else if (inventory.overWeightCoefficient == 3)
        {
            speedModifier = 0.1f;
            staminaModifier = 0.1f;
        }
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
