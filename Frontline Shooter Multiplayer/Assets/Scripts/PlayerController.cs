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
    [SerializeField] private GameObject bulletImpact;

    [SerializeField] private float shotCounter;   //Visible shot value 
    private Camera cam;

    [SerializeField] private float maxHeat, coolRate, overHeatCoolRate;
    private float heatCounter;
    private bool overHeated;

    public Gun[] allGuns;
    private int selectedGun;

    [SerializeField] private float muzzleDisplayTime;    //for showing irr of frame rate
    private float muzzleCounter;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        cam = Camera.main;
        UIController.instance.weaponTemperatureSlider.maxValue = maxHeat;
        switchGuns();
    }

    private void Update()
    {
        playerRotation();
        playerInputs();
        if (invertLook)
        {
            lookUpDown();
        }
        else
        {
            lookUpDownInverted();
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
            if (Input.GetMouseButtonDown(0))
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
    }

    private void playerInputs()
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

        isShooting();
        cursorLockUnlock();
    }

    private void shoot()
    {
        Ray ray = cam.ViewportPointToRay(new Vector3 (0.5f, 0.5f, 0f));   // shoot at 0.5 camera angle in worldpoint
        ray.origin = cam.transform.position;  //from where a ray starts
        if(Physics.Raycast(ray, out RaycastHit hit))
        {
            GameObject bulletImpactObject =  Instantiate(bulletImpact, hit.point + (hit.normal * 0.002f), Quaternion.LookRotation(hit.normal, Vector3.up));
            Destroy(bulletImpactObject, 5f);
        }
        shotCounter = allGuns[selectedGun].timeBetweenShots;

        heatCounter += allGuns[selectedGun].heatPerShot;
        if(heatCounter >= maxHeat)
        {
            heatCounter = maxHeat;  // edge case
            overHeated = true;
            UIController.instance.overheatedMessage.gameObject.SetActive(true);
        }
        allGuns[selectedGun].muzzleFlash.SetActive(true);
        muzzleCounter = muzzleDisplayTime;
    }   

    private void isShooting()
    {
        if (allGuns[selectedGun].muzzleFlash.activeInHierarchy)
        {
            muzzleCounter -= Time.deltaTime;  //edge no go if offflash

            if(muzzleCounter <= 0)
            {
                allGuns[selectedGun].muzzleFlash.SetActive(false);
            }
        }
        if (!overHeated)
        {
            if (Input.GetMouseButtonDown(0))
            {
                shoot();
            }
            if (Input.GetMouseButton(0) && allGuns[selectedGun].isAutomatic)  //Automating firing rate
            {
                shotCounter -= Time.deltaTime;
                if (shotCounter <= 0)
                {
                    shoot();
                }
            }
            heatCounter -= coolRate * Time.deltaTime;   //How fast the meter cools down
        }
        else
        {
            heatCounter -= overHeatCoolRate * Time.deltaTime;
            if (heatCounter <= 0)
            {
                heatCounter = 0;
                overHeated = false;
                UIController.instance.overheatedMessage.gameObject.SetActive(false);
            }
        }

        if (heatCounter <= 0)
        {
            heatCounter = 0f;
        }

        UIController.instance.weaponTemperatureSlider.value = heatCounter;

        if (Input.GetAxisRaw("Mouse ScrollWheel") > 0f)
        {
            selectedGun++;

            if (selectedGun >= allGuns.Length)
            {
                selectedGun = 0;
            }
            switchGuns();
        }
        else if (Input.GetAxisRaw("Mouse ScrollWheel") < 0f)
        {
            selectedGun--;
            if (selectedGun < 0)
            {
                selectedGun = allGuns.Length - 1;
            }
            switchGuns();
        }
    }

    private void switchGuns()
    {
        foreach(Gun gun in allGuns)
        {
            gun.gameObject.SetActive(false);   //Deactivating 1 by 1
        }

        allGuns[selectedGun].gameObject.SetActive(true);
        allGuns[selectedGun].muzzleFlash.SetActive(false);    //in case of switch gun and fire at same time
    }

    private void LateUpdate()
    {
        cam.transform.position = viewPoint.position;
        cam.transform.rotation = viewPoint.rotation;
    }


}
