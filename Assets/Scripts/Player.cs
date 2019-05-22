using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

[RequireComponent(typeof(PlayerSetup))]
public class Player : NetworkBehaviour {

	[SyncVar]
	private bool _isDead = false;
	public bool isDead {
		get { return _isDead; } 
		protected set { _isDead = value; }
	}

	[SerializeField]
	private int maxHealth = 100;

	[SyncVar]
	private int currentHealth;

	[SerializeField]
	private Behaviour[] disabledOnDeath;
	private bool[] wasEnabled;

	[SerializeField]
	private GameObject[] disabledGameObjectsOnDeath;

	[SerializeField]
	private GameObject deathEffect;

	[SerializeField]
	private GameObject spawnEffect;

	private bool firstSetup = true;


	[Command]
	public void CmdPlayerSetup () {
		if (isLocalPlayer) {
			// Switch camera
			GameManager.instance.SetSceneCameraActive (false);
			GetComponent<PlayerSetup> ().playerUIInstance.SetActive (true);
		}

		CmdBroadcastNewPlayerSetup ();
	}

	private void CmdBroadcastNewPlayerSetup () {
		RpcSetupPlayerOnAllClients ();
	}

	[ClientRpc]
	private void RpcSetupPlayerOnAllClients () {
		if (firstSetup) {
			wasEnabled = new bool[disabledOnDeath.Length];
			for (int i = 0; i < wasEnabled.Length; i++) {
				wasEnabled [i] = disabledOnDeath [i].enabled;

			}

			firstSetup = false;
		}

		SetDefaults ();

	}

	void Update () {
		if (!isLocalPlayer)
			return;

		if (Input.GetKeyDown (KeyCode.K))
			RpcTakeDamage (999);
	}

	[ClientRpc]
	public void RpcTakeDamage(int _damage){
		if (isDead)
			return;
		
		currentHealth -= _damage;

		Debug.Log (transform.name + " now has " + currentHealth + "/" + maxHealth);

		if (currentHealth <= 0) {
			Die ();
		}
	}

	private void Die() {
		isDead = true;

		// Disable all necessary componenets
		for (int i=0; i < disabledOnDeath.Length; i++){
			disabledOnDeath[i].enabled = false;
		}

		// Disable all necessary player GFX gameobjects
		for (int i=0; i < disabledGameObjectsOnDeath.Length; i++){
			disabledGameObjectsOnDeath[i].SetActive(false);
		}

		// Special condition to handle disabling the collider;
		Collider _col = GetComponent<Collider>();
		if (_col != null)
			_col.enabled = false;

		// Switch camera
		if (isLocalPlayer) {
			GameManager.instance.SetSceneCameraActive (true);
			GetComponent<PlayerSetup> ().playerUIInstance.SetActive(false);
		}
			
		Debug.Log (transform.name + " is dead");

		// Spawn a death effect
		GameObject _gfxIns = (GameObject)Instantiate(deathEffect, transform.position, Quaternion.identity);
		Destroy (_gfxIns, 3f);

		StartCoroutine (Respawn ());
	}

	private IEnumerator Respawn() {
		yield return new WaitForSeconds (GameManager.instance.matchSettings.respawnTime);

		Transform _spawnPoint = NetworkManager.singleton.GetStartPosition ();
		transform.position = _spawnPoint.position;
		transform.rotation = _spawnPoint.rotation;

		yield return new WaitForSeconds (0.1f);

		CmdPlayerSetup ();

		Debug.Log (transform.name + " respawned.");

	}

	public void SetDefaults(){
		isDead = false;

		currentHealth = maxHealth;

		// Turn on all components that might be disabled
		for (int i=0; i <  disabledOnDeath.Length; i++){
			disabledOnDeath[i].enabled = wasEnabled[i];
		}

		// Turn on all necessary player GFX gameobjects that might be disabled
		for (int i=0; i < disabledGameObjectsOnDeath.Length; i++){
			disabledGameObjectsOnDeath[i].SetActive(true);
		}

		// Special condition to handle enabling the collider;
		Collider _col = GetComponent<Collider>();
		if (_col != null)
			_col.enabled = true;

		// Spawn a spawn effect
		GameObject _gfxIns = (GameObject)Instantiate(spawnEffect, transform.position, Quaternion.identity);
		Destroy (_gfxIns, 3f);

	}

}
