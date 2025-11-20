using DG.Tweening;
using Unity.XR.CoreUtils;
using UnityEngine;

public class GhostWeapon : Actor
{
    [SerializeField] private WeaponType _weaponType;

    [Header("Tweening")]
    [SerializeField] private Vector3 _endPos;

    private const float CYCLE_LENGTH = 2f;

    protected override void Start()
    {
        base.Start();

        transform.DOLocalMove(_endPos, CYCLE_LENGTH)
                 .SetEase(Ease.InOutSine)
                 .SetLoops(-1, LoopType.Yoyo);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Left Hand Physics") || 
            other.gameObject.layer == LayerMask.NameToLayer("Right Hand Physics"))
        {
            _logger.Log($"{other.gameObject.layer} has hit", _isDevMode);

            if (_weaponType == WeaponType.SWORD)
            {
                // GDDManager.Instance.EnableSword(true);
                _logger.Log($"{_weaponType} spawned", _isDevMode);
            }
            else
            {
                // GDDManager.Instance.EnableHammer(true);
            }

            gameObject.SetActive(false);
        }
    }
}
