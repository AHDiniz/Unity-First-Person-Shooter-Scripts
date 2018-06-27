using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerController
{
	[RequireComponent(typeof(CharacterController))]
	public class PlayerController : MonoBehaviour
	{
		[Header("Properties")]
		[Tooltip("The speed that the player will move while walking")]
		public float walkSpeed = 5;
		[Tooltip("The speed that the player will move while running")]
		public float runSpeed = 15;
		[Tooltip("The amount of force that the player will apply while jumping")]
		public float jumpForce = 20;

        float verticalVelocity = 0; // Holds the current value of the vertical speed
		CharacterController controller; // The CharacterController component reference holder

        private void Awake()
		{
			controller = GetComponent<CharacterController>(); // Getting the reference to the CharacterController component
		}

		private void Update()
		{
			PlayerMovement(); // Moving the player
		}

		private void PlayerMovement() // Function that makes the player move
		{
            float speed = (Input.GetButton("Fire3") && controller.isGrounded) ? runSpeed : walkSpeed; // Getting the desired movement speed

            // Getting the player input for the movement:
            float strafe = Input.GetAxis("Horizontal") * speed;
			float forward = Input.GetAxis("Vertical") * speed;

			Vector3 movement = new Vector3(strafe, 0, forward); // Creating the movement vector

			CalculateJumpMovement(ref movement); // Calculating the jump movement vector

			controller.Move(transform.rotation * movement * Time.deltaTime); // Moving the character
		}

		private void CalculateJumpMovement(ref Vector3 movement) // Function that calculates the jump movement vector with a given vector
		{
			verticalVelocity += Physics.gravity.y * Time.deltaTime;
			if (Input.GetButton("Jump") && controller.isGrounded)
			{
				verticalVelocity = jumpForce;
			}
            movement.y = verticalVelocity;
		}
	}
}
