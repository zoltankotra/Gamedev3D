using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Player : MonoBehaviour
{
    private CharacterController controller;
    public Transform cameraChild;


    private Vector3 playerVelocity;
    public float playerSpeed = 2.0f;
    public float jumpHeight = 1.0f;
    private float gravityValue = -9.81f;

    public float mouseSensitivity;
    private float mouseYValue = 0;

    private bool groundedPlayer;
    private bool canJump;
    public Transform groundCheck;
    public float groundDistance = 0.2f;
    public LayerMask groundMask;


    private void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        canJump = true;
    }

    void Update()
    {
        groundedPlayer = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = -2f;
        }

        Vector3 move = (Input.GetAxis("Horizontal") * transform.right + Input.GetAxis("Vertical") * transform.forward);
        move = move.magnitude > 1 ? move.normalized : move;

        controller.Move(move * Time.deltaTime * playerSpeed);

        // Makes the player jump
        if (Input.GetButton("Jump") && canJump && groundedPlayer)
        {
            StartCoroutine(JustJumped());
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -2.0f * gravityValue);
        }

        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);

        float mouseX = Input.GetAxis("Mouse X") * Time.deltaTime * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * Time.deltaTime * mouseSensitivity;

        mouseYValue -= mouseY;
        mouseYValue = Mathf.Clamp(mouseYValue,-90f, 90f);

        transform.Rotate(Vector3.up, mouseX);
        cameraChild.localRotation = Quaternion.Euler(mouseYValue, 0, 0);
    }

    public void SetMouseSensitivity(float sensitivity)
    {
        mouseSensitivity = (sensitivity + 5) * 10;
    }

    IEnumerator JustJumped()
    {
        canJump = false;
        
        yield return new WaitForSeconds(0.5f);

        canJump = true;
    }
}
