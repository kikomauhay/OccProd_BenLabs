using UnityEngine;

public class WeaponSpawner : Actor
{
	[Header("Weapon")]
	[SerializeField] private GameObject[] _weapons;

	public void Enable(string weaponName)
	{
		if (GDDManager.Instance.PreferredWeapon == weaponName)
		{
			foreach (var item in _weapons)
				item.SetActive(weaponName == item.name);

            _logger.Log($"Spawned {weaponName}", _isDevMode);
        }
        else
		{
            _logger.Log("No weapon was made!", _isDevMode);
            //GDDManager.Instance.Retry()? Or whatever fucking function to reset the canvas
        }

        _logger.Log("Enabled a weapon!", gameObject, TextColor.YELLOW, _isDevMode);
	}
}
