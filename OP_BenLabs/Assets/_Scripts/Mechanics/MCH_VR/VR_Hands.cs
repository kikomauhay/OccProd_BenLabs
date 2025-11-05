using UnityEngine;
using UnityEngine.InputSystem;

public class VR_Hands : MonoBehaviour
{
    #region Members

    public InputActionProperty PinchAnimationAction;
    public InputActionProperty GripAnimationAction;
    public Animator HandAnimator;
    public Transform TargetController;

    private float _triggerValue;
    private float _selectValue;
    private Rigidbody _rb;
    private Quaternion _rotationDifference;
    private Collider[] _handColliders;

    #endregion

    #region Unity

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _handColliders = GetComponentsInChildren<Collider>();
    }

    #endregion

    #region Public

    public void DelayedEnableOnHandCollider(float delay)
    {
        Invoke("EnableHandCollider", delay);
    }

    public void DisableHandCollider()
    {
        if (this == null || gameObject == null) return;
        if (_handColliders == null || _handColliders.Length == 0) return;

        foreach (var item in _handColliders)
        {
            item.enabled = false; // timing issue => gets called when it's already deleted
        }    
    }

    #endregion

    #region Private
    private void EnableHandCollider()
    {
        if (this == null || gameObject == null) return;
        if (_handColliders == null || _handColliders.Length == 0) return;

        foreach (var item in _handColliders)
        {
            item.enabled = true;
        }
    }

    private void Update()
    {
        _triggerValue = PinchAnimationAction.action.ReadValue<float>();
        _selectValue = GripAnimationAction.action.ReadValue<float>();

        HandAnimator.SetFloat("Trigger",_triggerValue);
        HandAnimator.SetFloat("Grip", _selectValue);
    }

    private void FixedUpdate()
    {
        _rb.velocity = (TargetController.position - transform.position)/Time.fixedDeltaTime;

        _rotationDifference = TargetController.rotation * Quaternion.Inverse(transform.rotation);
        _rotationDifference.ToAngleAxis(out float _angleInDegree, out Vector3 _rotationAxis);
        Vector3 _rotationDifferenceInDegree = _angleInDegree * _rotationAxis;
        _rb.angularVelocity = (_rotationDifferenceInDegree * Mathf.Deg2Rad / Time.fixedDeltaTime);
    }

    #endregion
}
