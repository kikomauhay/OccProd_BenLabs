using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSpawner : MonoBehaviour
{
    [SerializeField] private List<GameObject> weapons;

    public void Spawn(string _weaponName)
    {
        foreach(var item in weapons) 
        { 
          item.SetActive(_weaponName == item.name);
        }
    }
}
