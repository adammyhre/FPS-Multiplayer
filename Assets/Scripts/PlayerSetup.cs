using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Player))]
[RequireComponent(typeof(PlayerController))]
public class PlayerSetup : NetworkBehaviour {

	[SerializeField]
	Behaviour[] componentsToDisable;

	[SerializeField]
	string remoteLayerName = "RemotePlayer";

	[SerializeField]
	string dontDrawLayerName = "DontDraw";
	[SerializeField]
	GameObject playerGFX;

	[SerializeField]
	GameObject playerUIPrefab;
	[HideInInspector]
	public GameObject playerUIInstance;

	void Start () {
		if (!isLocalPlayer) {
			DisableComponents ();
			AssignRemoteLayer ();
		}
		else {
			// Disable Player GFX for local player
			SetLayerRecursively(playerGFX, LayerMask.NameToLayer(dontDrawLayerName));
		
			playerUIInstance = Instantiate (playerUIPrefab);
			playerUIInstance.name = playerUIPrefab.name;

			// Configure player UI
			PlayerUI ui = playerUIInstance.GetComponent<PlayerUI>();
			if (ui == null) {
				Debug.LogError ("No ui componenet on PlayerUI prefab)");
			}
			ui.SetController (GetComponent<PlayerController> ());

			GetComponent<Player>().CmdPlayerSetup ();

		}

	}

	void SetLayerRecursively(GameObject obj, int newLayer) {
		obj.layer = newLayer;

		foreach (Transform child in obj.transform) {
			SetLayerRecursively (child.gameObject, newLayer);
		}
	}

	public override void OnStartClient () {
		base.OnStartClient();

		string _netID = GetComponent<NetworkIdentity>().netId.ToString();
		Player _player = GetComponent<Player>();

		GameManager.RegisterPlayer(_netID, _player);
	
	}

	void AssignRemoteLayer () {
		//Debug.Log (remoteLayerName);
		gameObject.layer = LayerMask.NameToLayer (remoteLayerName);
	}

	void DisableComponents () {
		for (int i = 0; i < componentsToDisable.Length; i++) {
			componentsToDisable [i].enabled = false;

		} 

	}

	void OnDisable() {
		Destroy (playerUIInstance);

		if (isLocalPlayer)
			GameManager.instance.SetSceneCameraActive (true);

		GameManager.UnRegisterPlayer(transform.name);

	}
		

}
