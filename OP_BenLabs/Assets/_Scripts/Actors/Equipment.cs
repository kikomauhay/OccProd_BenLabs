using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(SoundEmitter))]
public class Equipment : Actor
{
    #region SerializeField

    [Header("SFX")]
    [SerializeField] protected Sound _pickUpSFX;
    [SerializeField] protected Sound _landOnFloorSFX;
 
    #endregion
    #region Protected

    protected Renderer _rend;
    protected Vector3 _startPosition;
    protected Quaternion _startRotation;
    
    protected GameManager _gameMgr = GameManager.Instance;
    protected SoundEmitter _soundEmitter;

    #endregion
    
    #region Unity
    
    protected override void OnEnable()
    {
        _gameMgr.OnBarGameStart += ResetPosition;
    }
    protected override void OnDisable()
    {
        _gameMgr.OnBarGameStart -= ResetPosition;
    }
    protected override void Start()
    {
        base.Start(); // already contains both Init methods


    }

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
            _logger.Log($"{name}'s position has been reset!", ColorType.YELLOW);
    }
    
    #endregion
    #region Helpers

    protected override void InitComponents()
    {
        _rend = GetComponent<MeshRenderer>();
        _soundEmitter = GetComponent<SoundEmitter>();
    }
    protected override void InitVariables()
    {
        _startPosition = transform.position;
        _startRotation = transform.rotation;
    }

    #endregion
}
