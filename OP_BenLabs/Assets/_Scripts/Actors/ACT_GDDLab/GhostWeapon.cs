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
        if (other.gameObject.layer == LayerMask.NameToLayer("Left Hand Physics")
            || other.gameObject.layer == LayerMask.NameToLayer("Right Hand Physics"))
        {
            Debug.Log($"{other.gameObject.layer} has hit");

            if (_weaponType == WeaponType.SWORD)
            {
                GDDManager.Instance.ActivateSword();
                Debug.Log($"{_weaponType} spawned");
                //_logger.Log("Activated sword", _isDevMode);
            }
            else
            {
                GDDManager.Instance.ActivateHammer();
                //_logger.Log("Activated hammer", _isDevMode);
            }

            gameObject.SetActive(false);
        }
    }
}
