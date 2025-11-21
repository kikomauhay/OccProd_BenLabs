
using UnityEngine;

public class Weapon : Actor
{
    #region Members

    public WeaponType WeaponType => _weaponType;
    public float Damage => _dmg;
    public float DamageModifier { get; set; }
    private bool _isBuffed;

    [Header("Weapon Stats")]
    [SerializeField] private WeaponType _weaponType;
    [SerializeField] private float _dmg;

    #endregion

    #region Methods

    protected override void OnEnable()
    {
        GDDManager.Instance.OnBuffWeapon += EVENT_IncreaseDamage;
    }
    protected override void OnDisable() 
    {
        GDDManager.Instance.OnBuffWeapon -= EVENT_IncreaseDamage;

        if (_isBuffed)
        {
            DamageModifier = 0f;
            _isBuffed = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Enemy>())
        {
            Enemy e = other.GetComponent<Enemy>();

            e.TakeDamage(_dmg+ DamageModifier);
            _logger.Log($"{name} dealt damage!", _isDevMode);
        }
    }

    private void EVENT_IncreaseDamage() 
    {
        if (_isBuffed)
        {
            _logger.Log($"{name} has already been buuffed!", _isDevMode);
            return;
        }

        DamageModifier = 10f;
        _isBuffed = true;

        _logger.Log($"{name} total damage: {_dmg} + {DamageModifier}", _isDevMode);
    }

    protected override void InitVariables()
    {
        _isBuffed = false;
    }

    #endregion
}

public enum WeaponType
{
    DEFAULT = -1,
    SWORD = 0,
    HAMMER = 1
}