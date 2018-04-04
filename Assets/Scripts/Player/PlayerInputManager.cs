using UnityEngine;

public class PlayerInputManager : MonoBehaviour
{
	public PlayerInput Current;

	void Update()
	{
		Vector3 moveInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
		Vector2 mouseInput = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));

		bool jumpInput = Input.GetButtonDown("Jump");
		bool crouchInput = Input.GetButton("Crouch");
		bool primaryFireInput = Input.GetButtonDown("PrimaryFire");
		bool secondaryFireInput = Input.GetButton("SecondaryFire");
		bool interactInput = Input.GetButtonDown("Interact");

		Current = new PlayerInput()
		{
			MoveInput = moveInput,
			MouseInput = mouseInput,
			JumpInput = jumpInput,
			CrouchInput = crouchInput,
			PrimaryFireInput = primaryFireInput,
			SecondaryFireInput = secondaryFireInput,
			InteractInput = interactInput
		};
	}

	public struct PlayerInput
	{
		public Vector3 MoveInput;
		public Vector2 MouseInput;
		public bool JumpInput;
		public bool CrouchInput;
		public bool PrimaryFireInput;
		public bool SecondaryFireInput;
		public bool InteractInput;
	}
}
