using UnityEngine;

//[CreateAssetMenu(menuName = "Scriptable Objects/Weapon")]
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

    private void OnEnable()
    {
        GDDManager.Instance.OnBuffWeapon += EVENT_IncreaseDamage;
    }
    private void OnDisable()
    {
        GDDManager.Instance.OnBuffWeapon -= EVENT_IncreaseDamage;
    }
        
    #endregion
    #region Private

    private void EVENT_IncreaseDamage() => DamageModifier = 10f;
        
    #endregion
}

public enum WeaponType
{
    SWORD = 0,
    HAMMER = 1,
    RAPIER = 2
}