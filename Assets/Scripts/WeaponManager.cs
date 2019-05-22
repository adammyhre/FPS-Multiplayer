using UnityEngine;
using UnityEngine.Networking;

public class WeaponManager : NetworkBehaviour {

	[SerializeField]
	private string weaponLayerName = "Weapon";

	[SerializeField]
	private Transform weaponHolder;

	[SerializeField]
	private PlayerWeapon primaryWeapon; // Primary (starting) weapon

	private PlayerWeapon currentWeapon; // Equipped weapon
	private WeaponGFX currentWeaponGFX;


	void Start () {
		EquipWeapon (primaryWeapon);
	}

	public PlayerWeapon GetCurrentWeapon () {
		return currentWeapon;
	}

	public WeaponGFX GetCurrentWeaponGFX () {
		return currentWeaponGFX;
	}

	void EquipWeapon (PlayerWeapon _weapon) {
		currentWeapon = _weapon;

		GameObject _weaponIns = Instantiate (_weapon.weaponGFX, weaponHolder.position, weaponHolder.rotation);
		_weaponIns.transform.SetParent (weaponHolder);

		currentWeaponGFX = _weaponIns.GetComponent<WeaponGFX> ();
		if (currentWeaponGFX == null)
			Debug.LogError("No weapon GFX on weapon component " + _weaponIns.name);
		
		if (isLocalPlayer) {
			Util.SetLayerRecursively (_weaponIns, LayerMask.NameToLayer (weaponLayerName));
		}
	}

}
