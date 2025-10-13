using System.Collections.Generic;
using UnityEngine;

public class WeaponSpawner : Actor
{
	[Header("Weapons")]
	[SerializeField] private List<GameObject> _weaponsList;

	public void Enable(string weaponName)
	{
		foreach (var item in _weaponsList)
			item.SetActive(weaponName == item.name);

        if (_isDevMode)
            _logger.Log("Enabled a weapon!", gameObject, ColorType.YELLOW);	
	}
}
