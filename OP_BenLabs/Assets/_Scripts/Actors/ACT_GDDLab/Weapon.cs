
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine;

[RequireComponent(typeof(SoundEmitter))]

public class Weapon : Actor
{
    #region Members

    [Header("Weapon Stats")]
    [SerializeField] private WeaponType _weaponType;
    [SerializeField] private float _dmg;
    [SerializeField] private Sound[] _hitSFXs;

    [Header("XR Controller Settings")]
    [SerializeField] private XRBaseController _controller;
    [SerializeField] private float _amplitude, _duration;

    private SoundEmitter _soundEmitter;
    private float _damageModifier;
    private bool _isBuffed;

    #endregion

    #region Unity

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Enemy>())
        {
            Enemy e = other.GetComponent<Enemy>();

            TriggerHaptic();
            e.TakeDamage(_dmg + _damageModifier);

            _soundEmitter.PlaySound(_hitSFXs[Random.Range(0, _hitSFXs.Length)]);
            _logger.Log($"{name} dealt damage!", _isDevMode);
        }
    }

    #endregion
    #region Public

    public void BuffWeapon()
    {
        _damageModifier = 10;
        _isBuffed = true;
    }
    public void ResetWeapon()
    {
        _damageModifier = 0;
        _isBuffed = false;
    }

    #endregion
    #region Private

    private void TriggerHaptic()
    {
        if (_controller == null) return;
        _controller.SendHapticImpulse(_amplitude, _duration);
    }

    #endregion
    #region Helpers

    protected override void InitComponents()
    {
        _soundEmitter = GetComponent<SoundEmitter>();
    }
    protected override void AssertComponents()
    {
        Debug.Assert(_weaponType != WeaponType.DEFAULT, "Using default value!", this);
        Debug.Assert(_dmg != 0, "_dmg is 0!", this);
        Debug.Assert(_hitSFXs.Length != 0, "Missing _hitSFXs elements!", this);

        Debug.Assert(_controller, "Missing _controller reference!", this);
        Debug.Assert(_amplitude != 0, "Missing _amplitude reference!", this);
        Debug.Assert(_duration != 0, "Missing _duration reference!", this);
    }
    protected override void InitVariables()
    {
        ResetWeapon();
    }

    #endregion
}

public enum WeaponType
{
    DEFAULT = -1,
    SWORD = 0,
    HAMMER = 1
}