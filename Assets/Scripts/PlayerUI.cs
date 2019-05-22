using UnityEngine;

public class PlayerUI : MonoBehaviour {

	[SerializeField]
	RectTransform thrusterFuelFill;

	[SerializeField]
	GameObject pauseMenu;

	private PlayerController controller;

	void Start () {
		PauseMenu.isOn = false;
	}

	public void SetController (PlayerController _controller){
		controller = _controller;
	}

	void SetFuelAmount(float _amount){
		thrusterFuelFill.localScale = new Vector3 (1f, _amount, 1f);
	}

	void Update () {
		SetFuelAmount (controller.GetThrusterFuelAmount());

		if (Input.GetKeyDown (KeyCode.Escape)) {
			TogglePauseMenu ();
		}
	}

	void TogglePauseMenu(){
		pauseMenu.SetActive (!pauseMenu.activeSelf);	
		PauseMenu.isOn = pauseMenu.activeSelf;
	}

}
