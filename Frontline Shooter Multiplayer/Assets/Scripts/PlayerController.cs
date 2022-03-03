using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Transform viewPoint;
    [SerializeField] private float mouseSensitivity;
    private float verticalRotStore;
    private Vector2 mouseInput;
    private bool isGrounded;
    [SerializeField] private bool invertLook;
    [SerializeField] private float moveSpeed, runSpeed, activeMoveSpeed;
    [SerializeField] private Vector3 moveDir, movement;
    [SerializeField] private CharacterController charCon;
    [SerializeField] private Transform groundCheckPoint;
    [SerializeField] private LayerMask groundLayers;
    [SerializeField] private float jumpForce, _gravityMod;
    private Camera cam;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        cam = Camera.main;
    }

    private void Update()
    {
        playerRotation();
        playerMove();
        if (invertLook)
        {
            lookUpDownInverted();
        }
        else
        {
            lookUpDown();
        }
    }

    private void lookUpDownInverted()
    {
        viewPoint.rotation = Quaternion.Euler(-verticalRotStore, viewPoint.rotation.eulerAngles.y,
                                                viewPoint.rotation.eulerAngles.z);
    }

    private void lookUpDown()
    {
        viewPoint.rotation = Quaternion.Euler(verticalRotStore, viewPoint.rotation.eulerAngles.y,
                                                viewPoint.rotation.eulerAngles.z);
    }

    private void playerRotation()
    {
        mouseInput = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y")) * mouseSensitivity;
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y + mouseInput.x,
                                                transform.rotation.eulerAngles.z);
        verticalRotStore += mouseInput.y;
        verticalRotStore = Mathf.Clamp(verticalRotStore, -60f, 60f);
    }

    private void playerJump()
    {
        if(Input.GetButtonDown ("Jump") && isGrounded)
        {
            movement.y = jumpForce;
        }
        isGrounded = Physics.Raycast(groundCheckPoint.position, Vector3.down, 0.25f, groundLayers);
    }

    private void cursorLockUnlock()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
        }
        else if (Cursor.lockState == CursorLockMode.None)
        {
            if (Input.GetButtonDown(0))
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
    }

    private void playerMove()
    {
        moveDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical"));
        if (Input.GetKey(KeyCode.LeftShift))
        {
            activeMoveSpeed = runSpeed;
        }
        else
        {
            activeMoveSpeed = moveSpeed;
        }
        float yVel = movement.y;  //polishing gravity
        movement = ((transform.forward * moveDir.z) + (transform.right * moveDir.x)).normalized * activeMoveSpeed; // to control fast diaognal movement
        movement.y = yVel;
        if (charCon.isGrounded)
        {
            movement.y = 0f;
        }
        playerJump();
        movement.y += Physics.gravity.y * Time.deltaTime * _gravityMod;
        charCon.Move(movement * Time.deltaTime);
        cursorLockUnlock();
    }

    private void LateUpdate()
    {
        cam.transform.position = viewPoint.position;
        cam.transform.rotation = viewPoint.rotation;
    }


}
