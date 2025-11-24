
using UnityEngine;

[RequireComponent(typeof(SoundEmitter))]
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

    [SerializeField] private Sound[] _hitSFXs;

    private SoundEmitter _soundEmitter;

    #endregion

    #region Unity

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
            _soundEmitter.PlaySound(_hitSFXs[Random.Range(0, _hitSFXs.Length)]);
            _logger.Log($"{name} dealt damage!", _isDevMode);
        }
    }

    #endregion
    #region Private

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

    #endregion
    #region Helpers

    protected override void AssertComponents()
    {
        Debug.Assert(_hitSFXs.Length != 0, "Missing _hitSFX elements!", this);
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