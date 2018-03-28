using UnityEngine;

public class PlayerInputManager : MonoBehaviour
{
	SuperCharacterController characterController;
	PlayerController playerController;
	PlayerStateMachine playerStateMachine;
	PlayerCamera playerCamera;

	public PlayerInput Current;

	void Awake()
	{
		characterController = GetComponent<SuperCharacterController>();
		playerController = GetComponent<PlayerController>();
		playerStateMachine = GetComponent<PlayerStateMachine>();
		playerCamera = Camera.main.GetComponent<PlayerCamera>();
	}

	void Update()
	{
		Vector3 moveInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
		Vector2 mouseInput = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));

		bool jumpInput = Input.GetButtonDown("Jump");
		bool crouchInput = Input.GetButton("Crouch");
		bool primaryFireInput = Input.GetButtonDown("PrimaryFire");
		bool interactInput = Input.GetButtonDown("Interact");

		Current = new PlayerInput()
		{
			MoveInput = moveInput,
			MouseInput = mouseInput,
			JumpInput = jumpInput,
			CrouchInput = crouchInput,
			PrimaryFireInput = primaryFireInput,
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
		public bool InteractInput;
	}
}
