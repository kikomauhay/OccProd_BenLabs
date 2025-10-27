using System.Collections.Generic;
using UnityEngine;

public class WeaponSpawner : Actor
{
	[Header("Weapons")]
	[SerializeField] private List<GameObject> _weaponsList;

	public void Enable(WeaponType weaponType)
	{
		foreach (GameObject weapon in _weaponsList)
			weapon.SetActive(weapon.GetComponent<Weapon>().WeaponType == weaponType);

        if (_isDevMode)
            _logger.Log($"Enabled {weaponType}!", gameObject, TextColor.YELLOW);	
	}
}
