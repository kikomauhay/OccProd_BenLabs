using UnityEngine;

public class WeaponSpawner : Actor
{
	[Header("Weapon")]
	[SerializeField] private GameObject[] _weapons;

	public void Enable(string weaponName)
	{
		if (GDDManager.Instance.GetPreferredWeapon() == weaponName)
		{
			foreach (var item in _weapons)
				item.SetActive(weaponName == item.name);
			Debug.Log($"Spawned {weaponName}");
		}
		else
		{
            _logger.Log($"{weaponName} has spawned. Wrong Weapon", _isDevMode);
            //GDDManager.Instance.Retry()? Or whatever fucking function to reset the canvas
        }

        _logger.Log("Enabled a weapon!", gameObject, TextColor.YELLOW, _isDevMode);
	}
}
