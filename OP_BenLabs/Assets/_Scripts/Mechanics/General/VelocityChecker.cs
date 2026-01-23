using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VelocityChecker : MonoBehaviour
{
    #region SerializeField

    [SerializeField] private float _magCheck;
    [SerializeField] private Rigidbody _rb;

    #endregion
    #region Public

    public bool MagIsValid;

    #endregion

    #region Unity

    private void Awake()
    {
        MagIsValid = false;
    }

    private void FixedUpdate()
    {
        if (_rb == null) return;

        if(_rb.velocity.magnitude > _magCheck)
        {
            MagIsValid=true;
        }
    }
    #endregion
}
