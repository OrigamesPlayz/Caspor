using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostMovement : MonoBehaviour
{
    public Animator ghostAnim;
    public Transform ghostArmature;
    private Quaternion savedLocalRotation;

    [Header("Movement")]
    public float moveSpeed = 10f;

    [Header("Keybinds")]
    public KeyCode flyUpKey = KeyCode.Space;
    public KeyCode flyUpKeyAlt = KeyCode.E;
    public KeyCode flyDownKey = KeyCode.LeftShift;
    public KeyCode flyDownKeyAlt = KeyCode.Q;

    [Header("Flight Settings")]
    public bool canFly = true;
    public float flySpeed = 10f;
    public float flySmoothness = 5f;

    public Transform orientation;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        savedLocalRotation = ghostArmature.localRotation;
    }

    private void Update()
    {
        MyInput();
        rb.drag = 0;

        HandleAnimations();
    }

    private void FixedUpdate()
    {
        if (canFly)
            FlyPlayer();
        else
            MovePlayer();
    }

    void LateUpdate()
    {
        ghostArmature.localRotation = Quaternion.Euler(savedLocalRotation.eulerAngles.x, ghostArmature.localEulerAngles.y, ghostArmature.localEulerAngles.z);
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
    }

    private void MovePlayer()
    {
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        SpeedControl();
    }

    private void FlyPlayer()
    {
        Vector3 flyDirection = (orientation.forward * verticalInput + orientation.right * horizontalInput).normalized;

        if (Input.GetKey(flyUpKey) || Input.GetKey(flyUpKeyAlt))
            flyDirection += Vector3.up;
        if (Input.GetKey(flyDownKey) || Input.GetKey(flyDownKeyAlt))
            flyDirection += Vector3.down;

        flyDirection = flyDirection.normalized;

        Vector3 targetVelocity = flyDirection * flySpeed;

        rb.velocity = Vector3.Lerp(rb.velocity, targetVelocity, Time.fixedDeltaTime * flySmoothness);
    }

    private void SpeedControl()
    {
        if (canFly) return;

        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        if (flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }

    private void HandleAnimations()
    {
        bool isPressingMoveKeys = horizontalInput != 0 || verticalInput != 0;
        ghostAnim.SetBool("isMoving", isPressingMoveKeys);
    }
}
