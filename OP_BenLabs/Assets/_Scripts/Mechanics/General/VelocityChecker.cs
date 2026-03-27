using UnityEngine;

public class VelocityChecker : Actor
{
    #region Members

    private Vector3 _lastPos;
    public float CurrentMagnitude;

    #endregion
    #region Methods

    protected override void InitVariables()
    {
        _lastPos = new();
    }

    private void FixedUpdate()
    {
        Vector3 velocity = (transform.position - _lastPos) / Time.fixedDeltaTime;
        CurrentMagnitude = velocity.magnitude;
        
        _lastPos = transform.position;
        a_logger.Log($"{CurrentMagnitude}", a_isDevMode);
    }
    #endregion
}
