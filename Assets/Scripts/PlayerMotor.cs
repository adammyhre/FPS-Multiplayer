using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMotor : MonoBehaviour {

	[SerializeField]
	private Camera cam;

	private Vector3 velocity = Vector3.zero;
	private Vector3 rotation = Vector3.zero;
	private float cameraRotationX = 0f;
	private float currentCameraRotationX = 0f;
	private Vector3 thrusterForce = Vector3.zero;

	[SerializeField]
	private float cameraRotationLimit = 85f;

	private Rigidbody rb;

	void Start () {
		rb = GetComponent<Rigidbody> ();

	}

	public void Move (Vector3 _velocity) {
		velocity = _velocity;

	}

	public void Rotate (Vector3 _rotation) {
		rotation = _rotation;

	}

	public void ApplyThruster (Vector3 _thrusterForce) {
		thrusterForce = _thrusterForce;
	}

	public void RotateCamera (float _cameraRotationX) {
		cameraRotationX = _cameraRotationX;

	}
		
	void FixedUpdate () {
		// Run every physics iteration
		PerformMovement ();
		PerformRotation ();

	}

	void PerformMovement() {
		// Perform a movment on velocity vector
		if (velocity != Vector3.zero) {
			rb.MovePosition (transform.position + velocity * Time.fixedDeltaTime);
		}

		if (thrusterForce != Vector3.zero) {
			// Adds a force ignoring mass
			rb.AddForce (thrusterForce * Time.fixedDeltaTime, ForceMode.Acceleration);
		}

	}

	void PerformRotation() {
		// Perform a movment on rotation vector
		rb.MoveRotation (transform.rotation * Quaternion.Euler (rotation));
		// Rotate camera
		if (cam != null) {
			// Set our rotation and clamp it, then apply rotation to the transform of our camera
			currentCameraRotationX -= cameraRotationX;
			currentCameraRotationX = Mathf.Clamp (currentCameraRotationX, -cameraRotationLimit, cameraRotationLimit);

			cam.transform.localEulerAngles = new Vector3 (currentCameraRotationX, 0, 0);
		}

	}

}




