using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Weapon")]
public class Weapon : ScriptableObject
{
    #region Properties

    public WeaponType WeaponType => _weaponType;
    public float Damage => _damage;

    #endregion
    #region SerializeField

    [Header("Weapon Stats")]
    [SerializeField] private WeaponType _weaponType;
    [SerializeField] private float _damage;

    #endregion
}

public enum WeaponType
{
    SWORD = 0,
    HAMMER = 1,
    RAPIER = 2
}