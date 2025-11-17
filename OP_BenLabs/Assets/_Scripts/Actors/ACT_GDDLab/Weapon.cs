
using UnityEngine;

public class Weapon : Actor
{
    #region Properties

    public WeaponType WeaponType => _weaponType;
    public float Damage => _dmg;
    public float DamageModifier { get; set; }

    #endregion
    #region SerializeField

    [Header("Weapon Stats")]
    [SerializeField] private WeaponType _weaponType;
    [SerializeField] private float _dmg;

    #endregion

    #region Unity

    protected override void OnEnable()
    {
        GDDManager.Instance.OnBuffWeapon += EVENT_IncreaseDamage;
    }
    protected override void OnDisable() 
    {
        GDDManager.Instance.OnBuffWeapon -= EVENT_IncreaseDamage;
    }

    #endregion
    #region Private

    private void EVENT_IncreaseDamage() 
    {
        DamageModifier = 10f;
        _logger.Log($"Total damage = {_dmg} + {DamageModifier}", _isDevMode);
    }

    #endregion
}

public enum WeaponType
{
    SWORD = 0,
    HAMMER = 1,
    RAPIER = 2
}