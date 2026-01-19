using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

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

    private SoundEmitter _sndEmitter;
    private float _damageModifier;

    #endregion

    #region Actor

    protected override void AssertComponents()
    {
        a_logger.AssertReference(_weaponType != WeaponType.Default);
        a_logger.AssertReference(_dmg != 0);
        a_logger.AssertReference(_hitSFXs.Length != 0);

        a_logger.AssertReference(_controller);
        a_logger.AssertReference(_amplitude != 0);
        a_logger.AssertReference(_duration != 0);
    }
    protected override void InitComponents()
    {
        _sndEmitter = GetComponent<SoundEmitter>();
    }
    protected override void InitVariables()
    {
        ResetWeapon();
    }

    #endregion
    #region Unity

    private void OnTriggerEnter(Collider other)
    {
        void TriggerHaptic()
        {
            if (_controller != null)
                _controller.SendHapticImpulse(_amplitude, _duration);
        }
        
        if (other.GetComponent<Enemy>())
        {
            Enemy e = other.GetComponent<Enemy>();

            TriggerHaptic();
            e.TakeDamage(_dmg + _damageModifier);

            _sndEmitter.PlaySound(_hitSFXs[Random.Range(0, _hitSFXs.Length)]);
            a_logger.Log($"{name} dealt damage!", a_isDevMode);
        }
    }

    #endregion
    #region Public

    public void BuffWeapon() => _damageModifier = 10f;
    public void ResetWeapon() => _damageModifier = 0f;

    #endregion
}

public enum WeaponType
{
    Default = -1,
    Sword = 0,
    Hammer = 1
}