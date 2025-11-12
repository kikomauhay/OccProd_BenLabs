using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;

public class EmptyWeapon : Actor
{
    [SerializeField] private WeaponType _weaponType;

    private void OnTriggerEnter(Collider other)
    {
        
        if(other.gameObject.GetComponent<XROrigin>())
        {
            if (_weaponType == WeaponType.SWORD)
            {
                GDDManager.Instance.ActivateSword();
                Debug.Log("Activated sword");  
            }
            else
            {
                Debug.Log("Activated hammer");
                GDDManager.Instance.ActivateHammer();
            }
            
        }

        this.gameObject.SetActive(false);
    }
}
