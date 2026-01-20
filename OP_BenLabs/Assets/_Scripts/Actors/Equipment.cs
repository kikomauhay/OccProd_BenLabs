using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(SoundEmitter), typeof(MeshRenderer))]
public class Equipment : Actor, IInteractable
{
    #region SerializeField

    [Header("SFX")]
    [SerializeField] protected Sound _pickUpSFX;
    [SerializeField] protected Sound _landOnFloorSFX;

    #endregion
    #region Protected

    protected GameManager _gameMgr;
    
    protected MeshRenderer _rend;
    protected Rigidbody _rb;
    protected SoundEmitter _soundEmitter;

    protected Vector3 _startPosition;
    protected Quaternion _startRotation;

    #endregion
    
    #region Public

    public virtual void INT_Interact() => _soundEmitter.PlaySound(_pickUpSFX);
        
    #endregion
    #region Protected

    protected void ResetPosition() 
    {
        transform.SetPositionAndRotation(_startPosition, _startRotation);
        //_soundEmitter.PlaySound(_landOnFloorSFX);

        _logger.Log($"{name}'s position has been reset!", TextColor.YELLOW, _isDevMode);
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
        _rb.useGravity = true;

        _startPosition = transform.position;
        _startRotation = transform.rotation;
    }

    #endregion
}
