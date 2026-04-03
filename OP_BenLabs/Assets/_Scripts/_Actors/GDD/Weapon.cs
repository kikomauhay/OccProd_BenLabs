using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(Rigidbody), typeof(SoundEmitter), typeof(VelocityChecker))]

public class Weapon : Actor
{
    #region Inspector

    [Header("Weapon Stats")]
    [SerializeField] private WeaponType _weaponType;

    [Header("XR Controller Settings")]
    [SerializeField] private XRBaseController _controller;
    

    [Header("SFXs")]
    [SerializeField] private Sound[] _hitSFXs;

    [Header("VFXs")]
    [SerializeField] private GameObject _velocityVFX;
    
    #endregion
    #region Private

    private const float VELOCITY_AMPLITUDE = 0.3f;
    private const float VELOCITY_DURATION = 0.2f;
    private const float VELOCITY_THRESHOLD = 20f;

    private Rigidbody _rb;
    private SoundEmitter _sndEmitter;
    private VelocityChecker _velocityChecker;

    private float _currDmg;
    private float _dmgModifier;

    private Coroutine _vfxRoutine;

    #endregion

    #region Actor

    protected override void Test()
    {
        if (Input.GetKeyDown(KeyCode.Space)) _sndEmitter.PlayRandomSound(_hitSFXs);
    }
    protected override void AssertReferences()
    {
        a_logger.AssertReference(_weaponType != WeaponType.Default, this);

        a_logger.AssertReference(_controller != null, this);
        // a_logger.AssertReference(_amplitude != 0f, this);
        // a_logger.AssertReference(_duration != 0f, this);

        a_logger.AssertCollection(_hitSFXs, this);
    }
    protected override void InitComponents()
    {
        _rb = GetComponent<Rigidbody>();
        _sndEmitter = GetComponent<SoundEmitter>();
        _velocityChecker = GetComponent<VelocityChecker>();
    }
    protected override void InitVariables()
    {
        _rb.useGravity = false;
        _rb.isKinematic = false;

        ResetWeapon();
    }

    #endregion
    #region Unity

    private void FixedUpdate()
    {
        if (_velocityChecker.CurrentMagnitude > VELOCITY_THRESHOLD)
        {
            if (!_velocityVFX.activeSelf)
                _velocityVFX.SetActive(true);

            if (_vfxRoutine != null)
            {
                StopCoroutine(_vfxRoutine);
                _vfxRoutine = null;
            }
        }
        else
        {
            if (_vfxRoutine == null)
                _vfxRoutine = StartCoroutine(CO_DelayVFX());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        void CalculateDamage(float mag)
        {
            switch (_weaponType)
            {
                case WeaponType.Sword:
                    float swordDmg = mag switch
                    {
                        >= 23f => 12f,
                        _ => 6f
                    };
                    _currDmg = swordDmg;
                    break;
                
                case WeaponType.Hammer:
                    float hammerDmg = mag switch
                    {
                        >= 23f => 20f,
                        _ => 10f
                    };
                    _currDmg = hammerDmg;
                    break;
            }
        }
        
        if (other.GetComponent<Enemy>() == null)
        {
            a_logger.Log("There's no enemy component!", TextColor.Red, a_isDevMode);
            a_audMgr.PlayWrong();
            return;
        }
        if (_velocityChecker.CurrentMagnitude < VELOCITY_THRESHOLD) return;
        
        if (_controller != null)
            _controller.SendHapticImpulse(VELOCITY_AMPLITUDE, VELOCITY_DURATION);
        
        Enemy e = other.GetComponent<Enemy>();
        CalculateDamage(_velocityChecker.CurrentMagnitude);
        e.TakeDamage(_currDmg * _dmgModifier);

        _sndEmitter.PlayRandomSound(_hitSFXs);

        a_logger.Log($"{name} dealt damage!", a_isDevMode);
        a_logger.Log($"Current Velocity: <color={TextColor.Yellow}>{_velocityChecker.CurrentMagnitude}</color>", a_isDevMode);
    }

    #endregion
    #region Public

    public void BuffWeapon() => _dmgModifier = 1.25f;
    public void ResetWeapon() => _dmgModifier = 1f;

    #endregion

    private IEnumerator CO_DelayVFX()
    {
        yield return new WaitForSeconds(4f);

        _velocityVFX.SetActive(false);
        _vfxRoutine = null;
    }
}

public enum WeaponType
{
    Default = -1,
    Sword = 0,
    Hammer = 1
}