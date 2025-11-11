using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;

public class EmptyWeapon : Actor
{
    [SerializeField] private WeaponType _weaponType;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.GetComponent<XROrigin>() != null)
        {
            if(_weaponType == WeaponType.SWORD)
            {
                GDDManager.Instance.ActivateSword();
            }
            else
            {
                GDDManager.Instance.ActivateHammer();
            }
        }

        this.gameObject.SetActive(false);
    }
}
