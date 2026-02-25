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
    private float _maxMagnitude = 12.0f;
    private float _damageModifier;
    private float _dmg;

    #endregion

    #region Actor

    protected override void AssertReferences()
    {
        a_logger.AssertReference(_weaponType != WeaponType.Default, this);
        a_logger.AssertReference(_dmg != 0, this);
        a_logger.AssertReference(_hitSFXs.Length != 0, this);

        a_logger.AssertReference(_controller != null, this);
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
        void CalculateDamage(float mag)
        {
            switch (_weaponType)
            {
                case WeaponType.Sword:
                    float swordDmg = mag switch
                    {
                        >= 10f => 12f,
                        _ => 6f
                    };
                    _dmg = swordDmg;
                    break;
                
                case WeaponType.Hammer:
                    float hammerDmg = mag switch
                    {
                        >= 10f => 20f,
                        _ => 10f
                    };
                    _dmg = hammerDmg;
                    break;
            }
        }
        
        // float CalculateWeaponDamage(float magnitude)
        // {
        //     return _weaponType switch
        //     {
        //         WeaponType.Sword => magnitude switch
        //         {
        //             >= 10f => 12f,
        //             _ => 6f
        //         },
        //         WeaponType.Hammer => magnitude switch
        //         {
        //             >= 10f => 20f,
        //             _ => 10f
        //         },
        //         _ => 0f,
        //     };
        // }

        if (other.GetComponent<Enemy>() == null)
        {
            a_logger.Log("There's no enemy component!", TextColor.Red, a_isDevMode);
            a_audMgr.PlayWrong();
            return;
        }
        if (_velocityChecker.CurrentMagnitude < _maxMagnitude) return;
        
        if (_controller != null)
            _controller.SendHapticImpulse(_amplitude, _duration);
        
        Enemy e = other.GetComponent<Enemy>();
        CalculateDamage(_velocityChecker.CurrentMagnitude);
        e.TakeDamage(_dmg * _damageModifier);

        // new dmg checker with the new func (removes lines 108-110 if this works)
        // other.GetComponent<Enemy>().TakeDamage(CalculateWeaponDamage(_velocityChecker.CurrentMagnitude) * _damageModifier);

        _sndEmitter.PlayRandomSound(_hitSFXs);

        a_logger.Log($"{name} dealt damage!", a_isDevMode);
        a_logger.Log($"Current Velocity: <color={TextColor.Yellow}>{_velocityChecker.CurrentMagnitude}</color>", a_isDevMode);
    }

    #endregion
    #region Public

    public void BuffWeapon() => _damageModifier = 1.25f;
    public void ResetWeapon() => _damageModifier = 1f;

    #endregion
}

public enum WeaponType
{
    Default = -1,
    Sword = 0,
    Hammer = 1
}