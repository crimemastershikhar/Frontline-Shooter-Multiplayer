using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Transform viewPoint;
    [SerializeField] private float mouseSensitivity;
    private float verticalRotStore;
    private Vector2 mouseInput;
    [SerializeField] private bool invertLook;
    [SerializeField] private float moveSpeed, runSpeed, activeMoveSpeed;
    [SerializeField] private Vector3 moveDir, movement;
    [SerializeField] private CharacterController charCon;
    [SerializeField] private Camera cam;

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

    private void LateUpdate()
    {
        cam.transform.position = viewPoint.position;
        cam.transform.rotation = viewPoint.rotation;
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
        movement = ((transform.forward * moveDir.z) + (transform.right * moveDir.x)).normalized * activeMoveSpeed; // to control fast diaognal movement
        charCon.Move(movement * Time.deltaTime);
    }
}
