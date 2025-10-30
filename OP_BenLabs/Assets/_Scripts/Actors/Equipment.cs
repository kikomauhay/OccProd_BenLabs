using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(SoundEmitter), typeof(MeshRenderer))]
public class Equipment : Actor
{
    #region SerializeField

    [Header("SFX")]
    [SerializeField] protected Sound _pickUpSFX;
    [SerializeField] protected Sound _landOnFloorSFX;

    [Header("Testing")]
    [SerializeField] protected bool _useGravity;

    #endregion
    #region Protected

    protected GameManager _gameMgr;
    
    protected MeshRenderer _rend;
    protected Rigidbody _rb;
    protected SoundEmitter _soundEmitter;

    protected Vector3 _startPosition;
    protected Quaternion _startRotation;
    
    protected Shaker shakr;

    #endregion
    
    #region Public

    public void PickUpSound() => _soundEmitter.PlaySound(_pickUpSFX);
        
    #endregion
    #region Protected

    protected void ResetPosition() 
    {
        transform.SetPositionAndRotation(_startPosition, _startRotation);
        _soundEmitter.PlaySound(_landOnFloorSFX);

        if (_isDevMode)
            _logger.Log($"{name}'s position has been reset!", TextColor.YELLOW);
    }
    
    #endregion
    #region Helpers

    protected override void InitComponents()
    {
        _rend = GetComponent<MeshRenderer>();
        _rb = GetComponent<Rigidbody>();
        _soundEmitter = GetComponent<SoundEmitter>();
    }
    protected override void InitVariables()
    {
        _gameMgr = GameManager.Instance;

        _rend.enabled = true;
        
        _rb.angularDrag = 0f;
        _rb.useGravity = _useGravity;

        _startPosition = transform.position;
        _startRotation = transform.rotation;
    }

    #endregion
}
