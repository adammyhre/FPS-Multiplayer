using UnityEngine;

[RequireComponent(typeof(PlayerMotor))]
[RequireComponent(typeof(ConfigurableJoint))]
[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour {

	[SerializeField]
	private float speed = 5f;
	[SerializeField]
	private float lookSensetivity = 3f;

	[SerializeField]
	private float thrusterForce = 1500f;

	[SerializeField]
	private float thrusterFuelBurnSpeed = 1f;
	[SerializeField]
	private float thrusterFuelRegenSpeed = 0.3f;
	private float thrusterFuelAmount = 1f;

	public float GetThrusterFuelAmount() {
		return thrusterFuelAmount;
	}

	[SerializeField]
	private LayerMask environmentMask;

	[Header ("Spring Options:")]
	//[SerializeField]
	//private JointDriveMode jointMode = JointDriveMode.Position;
	[SerializeField]
	private float jointSpring = 20f;
	[SerializeField]
	private float jointMaxForce = 40f;

	// Component Caching
	private PlayerMotor motor;
	private ConfigurableJoint joint;
	private Animator animator;


	void Start() {
		motor = GetComponent<PlayerMotor> ();
		joint = GetComponent<ConfigurableJoint> ();
		animator = GetComponent<Animator> ();

		SetJointSettings (jointSpring);

	}

	void Update() {

		if (PauseMenu.isOn == true)
			return;

		// Set target position for spring, makes the physics act right when it comes to landing on objects and applying gravity
		RaycastHit _hit;

		if (Physics.Raycast (transform.position, Vector3.down, out _hit, 100f, environmentMask)) {
			joint.targetPosition = new Vector3 (0, -_hit.point.y, 0);
		} else {
			joint.targetPosition = new Vector3 (0, 0, 0);
		}
			
		// Calculate our movement (back, forward, side to side) velocity as a 3D vector
		float _xMov = Input.GetAxis("Horizontal");
		float _zMov = Input.GetAxis("Vertical");

		Vector3 _movHorizontal = transform.right * _xMov;
		Vector3 _movVertical = transform.forward * _zMov;
	
		// Final movement vector
		Vector3 _velocity = (_movHorizontal + _movVertical) * speed;

		// Animate movement
		animator.SetFloat("ForwardVelocity", _zMov);

		// Apply movement
		motor.Move(_velocity);


		// Calculate rotation as a 3D vector.  This is for turning the character around.
		float _yRot = Input.GetAxisRaw("Mouse X");
		Vector3 _rotation = new Vector3 (0f, _yRot, 0f) * lookSensetivity;
		// Apply character rotation
		motor.Rotate(_rotation);


		// Calculate camera rotation as a 3D vector.  This is for tilting the camera.
		float _xRot = Input.GetAxisRaw("Mouse Y");
		float _cameraRotationX = _xRot * lookSensetivity;
		// Apply camera rotation
		motor.RotateCamera(_cameraRotationX);

		// Calculate thruster force based on player input
		Vector3 _thrusterForce = Vector3.zero;
		if ((Input.GetButton ("Jump")) && (thrusterFuelAmount > 0)) {

			thrusterFuelAmount -= thrusterFuelBurnSpeed * Time.deltaTime;

			if (thrusterFuelAmount >= 0.01f) {
				_thrusterForce = Vector3.up * thrusterForce;
				SetJointSettings (0f);
			}

		} else {
			
			thrusterFuelAmount += thrusterFuelRegenSpeed * Time.deltaTime;

			SetJointSettings (jointSpring);

		}

		thrusterFuelAmount = Mathf.Clamp (thrusterFuelAmount, 0, 1);
		// Apply thruster force
		motor.ApplyThruster(_thrusterForce);

	}

	private void SetJointSettings(float _jointSpring) {
		// Manipulate the settings of the JoingDrive struct
		joint.yDrive = new JointDrive { 
			//mode = jointMode, 
			positionSpring = _jointSpring, 
			maximumForce = jointMaxForce 
		};	

	}

}
