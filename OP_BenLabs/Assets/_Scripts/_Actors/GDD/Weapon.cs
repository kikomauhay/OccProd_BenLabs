using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(SoundEmitter))]

public class Weapon : Actor
{
    #region Members

    [Header("Necessary Scripts")]
    [SerializeField] private VelocityChecker _velocityChecker;

    [Header("Weapon Stats")]
    [SerializeField] private WeaponType _weaponType;
    [SerializeField] private Sound[] _hitSFXs;

    [Header("XR Controller Settings")]
    [SerializeField] private XRBaseController _controller;
    [SerializeField] private float _amplitude, _duration;

    private SoundEmitter _sndEmitter;
    private float _damageModifier;
    private float _dmg;

    #endregion

    #region Actor

    protected override void AssertReferences()
    {
        a_logger.AssertReference(_weaponType != WeaponType.Default, this);
        a_logger.AssertReference(_dmg != 0, this);
        a_logger.AssertReference(_hitSFXs.Length != 0, this);

        a_logger.AssertReference(_controller, this);
        a_logger.AssertReference(_amplitude != 0, this);
        a_logger.AssertReference(_duration != 0, this);
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

        void CalcDamage(float mag)
        {
            switch (_weaponType)
            {
                case WeaponType.Sword:

                    float SwrdDmg = mag switch
                    {
                        >= 10f => 12f,
                        _ => 6f
                    };

                    SwrdDmg = _dmg;
                    break;
                case WeaponType.Hammer:

                    float HmrDmg = mag switch
                    {
                        >= 10f => 20f,
                        _ => 10f
                    };

                    HmrDmg = _dmg;
                    break;
            }
        }

        if (other.GetComponent<Enemy>())
        {
            if (_velocityChecker.CurrentMagnitude < 5F) return;
            Enemy e = other.GetComponent<Enemy>();

            TriggerHaptic();
            CalcDamage(_velocityChecker.CurrentMagnitude);
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