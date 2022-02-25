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
    [SerializeField] private float moveSpeed;
    [SerializeField] private Vector3 moveDir, movement;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        mouseInput = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y")) * mouseSensitivity;
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y + mouseInput.x,
                                                transform.rotation.eulerAngles.z);
        verticalRotStore +=  mouseInput.y;
        verticalRotStore = Mathf.Clamp(verticalRotStore, -60f, 60f);

        if(invertLook)
        {
            lookUpDownInverted();
        }
        else
        {
            lookUpDown();
        }

        playerMove();
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

    private void playerMove()
    {
        moveDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical"));
        movement = ((transform.forward * moveDir.z) + (transform.right * moveDir.x)).normalized; // to control fast diaognal movement
        transform.position += movement * moveSpeed * Time.deltaTime;
    }
}
