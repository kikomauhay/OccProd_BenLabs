using UnityEngine;

public class Weapon : Actor
{
    #region Properties

    public float Damage => _damage; 

    #endregion
    #region SerializeField

    [Header("Weapon Stats")]
    [SerializeField] protected WeaponType _weaponType;
    [SerializeField] protected float _atkSpeed, _damage;

    #endregion
    #region Private

    #endregion

    #region Unity

    protected override void Start()
    {
        base.Start(); // already contains both init methods        

    }

    #endregion
    #region Helpers

    protected override void InitComponents()
    {
        
    }
    protected override void InitVariables()
    {
        
    }

    #endregion
}

public enum WeaponType
{
    SWORD = 0,
    HAMMER = 1,
    RAPIER = 2
}