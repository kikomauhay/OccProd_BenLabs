using UnityEngine;

public class WeaponSpawner : Actor
{
	[Header("Weapon")]
	[SerializeField] private GameObject[] _weapons;

	public void Enable(string weaponName)
	{
		foreach (var item in _weapons)
			item.SetActive(weaponName == item.name);

		if (_isDevMode)
			_logger.Log("Enabled a weapon!", gameObject, TextColor.YELLOW);
	}
}
