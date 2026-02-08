using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VelocityChecker : MonoBehaviour
{
    #region SerializeField

    [SerializeField] Logger _logger;

    private Vector3 _lastPos;

    #endregion
    #region Public
    public float CurrentMagnitude;

    #endregion

    #region Unity

    private void FixedUpdate()
    {
        Vector3 velocity = (transform.position - _lastPos) / Time.fixedDeltaTime;
        CurrentMagnitude = velocity.magnitude;
        _lastPos = transform.position;

        //_logger.Log($"{CurrentMagnitude}",true);
    }
    #endregion
}
