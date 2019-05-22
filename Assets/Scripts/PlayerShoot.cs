using UnityEngine;
using UnityEngine.Networking;

[RequireComponent (typeof (WeaponManager))]
public class PlayerShoot : NetworkBehaviour {

	private const string PLAYER_TAG = "Player";

	[SerializeField]
	private Camera cam;

	[SerializeField]
	private LayerMask mask;

	[SerializeField]
	private PlayerWeapon currentWeapon;
	private WeaponManager weaponManager;

	void Start () {
		if (cam == null) {
			Debug.LogError ("PlayerShoot: No camera referenced");
			this.enabled = false;
		}

		weaponManager = GetComponent<WeaponManager> ();

	}

	void Update () {
		currentWeapon = weaponManager.GetCurrentWeapon ();

		if (PauseMenu.isOn == true)
			return;
		
		if (currentWeapon.fireRate <= 0f) {
			if (Input.GetButtonDown ("Fire1")) {
				Shoot ();
			}
		} else {
			if (Input.GetButtonDown ("Fire1")) {
				InvokeRepeating("Shoot", 0f, 1f/currentWeapon.fireRate);
			} else if (Input.GetButtonUp ("Fire1")) {
				CancelInvoke("Shoot");
			}
		}
	}

	// Called on the server when the player shoots
	[Command]
	void CmdOnShoot() {
		RpcDoShootEffect ();

	}

	// Called on all clients when we need a shoot effect
	[ClientRpc]
	void RpcDoShootEffect(){
		weaponManager.GetCurrentWeaponGFX().muzzleFlash.Play ();

	}

	// Called on the server when we hit something, point of impact and the normal of the surface
	[Command]
	void CmdOnHit(Vector3 _pos, Vector3 _normal){
		RpcDoHitEffect (_pos, _normal);

	}

	// Called on all clients to display hit effect
	[ClientRpc]
	void RpcDoHitEffect(Vector3 _pos, Vector3 _normal){
		// Instantiate a hit effect at the point of imapact, facing outwards towards the look rotation
		GameObject _hitEffect = (GameObject)Instantiate(weaponManager.GetCurrentWeaponGFX().impactEffect, _pos, Quaternion.LookRotation(_normal));
		Destroy (_hitEffect, 2f);
	}

	[Client]
	private void Shoot () {
		if (!isLocalPlayer)
			return;

		// We are shooting, call the OnShoot function on the server
		CmdOnShoot ();

		RaycastHit _hit;
		if (Physics.Raycast(cam.transform.position, cam.transform.forward, out _hit, currentWeapon.range, mask)) {
			// We hit something
			// Debug.Log("We hit " + _hit.collider.name);
			if (_hit.collider.tag == PLAYER_TAG) {
				CmdPlayerShot(_hit.collider.name, currentWeapon.damage);
			}

			// Call On Hit on the server
			CmdOnHit (_hit.point, _hit.normal);
		}

	}

	[Command]
	void CmdPlayerShot(string _playerID, int _damage) {
		Debug.Log(_playerID + " has been shot.");

		Player _player = GameManager.GetPlayer (_playerID);
		_player.RpcTakeDamage (_damage);
	}
}
