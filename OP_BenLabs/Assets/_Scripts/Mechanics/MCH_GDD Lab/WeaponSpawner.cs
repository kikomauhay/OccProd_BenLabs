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
		}
		else
		{
			Debug.LogWarning($"{weaponName} has spawned. Wrong Weapon");
            //GDDManager.Instance.Retry()? Or whatever fucking function to reset the canvas
        }


        if (_isDevMode)
			_logger.Log("Enabled a weapon!", gameObject, TextColor.YELLOW);
	}
}
