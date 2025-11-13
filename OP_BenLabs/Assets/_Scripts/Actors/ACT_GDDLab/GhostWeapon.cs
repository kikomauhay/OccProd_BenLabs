using DG.Tweening;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;

public class GhostWeapon : Actor
{
    [SerializeField] private WeaponType _weaponType;

    [Header("Tweening")]
    [SerializeField] private Vector3 _endPos;
    [SerializeField] private Quaternion _endRot;

    private const float CYCLE_LENGTH = 2f;

    protected override void Start()
    {
        base.Start();

        transform.DOLocalMove(_endPos, CYCLE_LENGTH)
                 .SetLoops(-1, LoopType.Yoyo)
                 .SetEase(Ease.InOutSine);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.GetComponentInChildren<XROrigin>()) return;

        if (_weaponType == WeaponType.SWORD)
        {
            GDDManager.Instance.ActivateSword();
            _logger.Log("Activated sword", _isDevMode);
        }
        else
        {
            GDDManager.Instance.ActivateHammer();
            _logger.Log("Activated hammer", _isDevMode);
        }

        gameObject.SetActive(false);
    }
}
