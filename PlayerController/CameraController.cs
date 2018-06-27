using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerController
{
    // This is going to control the camera movement with a giving mouse input
    public class CameraController : MonoBehaviour
    {
        [Header("References")]
        [Tooltip("The player's body transform component")]
        public Transform playerBody;
        [Space]

        [Header("Rotation")]
        [Tooltip("This is the speed that the game will respond the mouse input")]
        public float mouseSensitivity = 3;
        [Tooltip("The maximum limit that the camera can rotate vertically")]
        public float verticalRotLimit = 80;
        [Space]

        [Header("Head Shake")]
        [Tooltip("The speed that the head will shake while the character is walking")]
        public float walkBobbingSpeed = .18f;
        [Tooltip("The speed that the head will shake while the character is running")]
        public float runBobbingSpeed = .36f;
        [Tooltip("The height that the head will shake")]
        public float bobbingHeight = .2f;
        [Tooltip("The original position of the head")]
        public float midpoint = 1.8f;

        float xAxisClamp = 0; // Auxiliar that will help in limiting the vertical rotation
        float timer = 0; // Auxiliar variable that will help in the head shake method

        private void Start()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        private void Update()
        {
            RotateCamera(); // Rotating the camera
            HeadShake(); // Shaking the camera while walking/running
        }

        // Method that makes the camera rotate with a giving mouse input:
        private void RotateCamera()
        {
            // Getting the mouse inputs:
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");

            // Calculating the rotation amount in each axis:
            float rotAmountX = mouseX * mouseSensitivity;
            float rotAmountY = mouseY * mouseSensitivity;

            xAxisClamp -= rotAmountY; // Adding the vertical rotation amount to the clamp helper

            Vector3 targetRotCam = transform.rotation.eulerAngles; // Getting the current camera rotation data in degrees
            Vector3 targetRotBody = playerBody.transform.rotation.eulerAngles; // Getting the current body rotation data in degrees

            // Calculating the target rotation:
            targetRotCam.x -= rotAmountY;
            targetRotCam.z = 0;
            targetRotBody.y += rotAmountX;

            // Limiting the vertical rotation:
            if (xAxisClamp > verticalRotLimit)
                xAxisClamp = targetRotCam.x = verticalRotLimit;
            else if (xAxisClamp < -verticalRotLimit)
            {
                xAxisClamp = -verticalRotLimit;
                targetRotCam.x = 360 - verticalRotLimit;
            }

            transform.rotation = Quaternion.Euler(targetRotCam); // Putting the calculated rotation value in the camera's rotation
            playerBody.transform.rotation = Quaternion.Euler(targetRotBody); // Putting the calculated rotation value in the body's rotation
        }

        private void HeadShake()
        {
            float waveSlice = 0; // This will hold the wave movement that that camera should do

            // Getting the movement input:
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            float bobbingSpeed = (Input.GetButtonDown("Fire3")) ? runBobbingSpeed : walkBobbingSpeed;

            Vector3 pos = transform.localPosition; // Getting the current camera position

            // Shutting down the timer if there's no movement:
            if (Mathf.Abs(horizontal) == 0 && Mathf.Abs(vertical) == 0)
                timer = 0;
            else // If there's movement input:
            {
                waveSlice = Mathf.Sin(timer); // Getting the sin of the timer
                timer += bobbingSpeed; // Adding the current speed to the timer
                if (timer > Mathf.PI * 2) // Limiting the timer value to the sin interval
                    timer -= Mathf.PI * 2;
            }

            if (waveSlice != 0) // If there should be a difference to the head position
            {
                float translateChange = waveSlice * bobbingHeight; // Calculating the height where the head should be
                
                // Applying the current movement input to the head height:
                float totalAxes = Mathf.Abs(horizontal) + Mathf.Abs(vertical);
                totalAxes = Mathf.Clamp(totalAxes, 0, 1);
                translateChange *= totalAxes;
                pos.y = midpoint + translateChange;
            }
            else
                pos.y = midpoint; // If the character is still, then the head is in the original position

            transform.localPosition = pos; // Applying the change to the head
        }
    }
}