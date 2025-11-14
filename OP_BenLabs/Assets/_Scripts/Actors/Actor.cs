using UnityEngine;

public abstract class Actor : MonoBehaviour
{
    #region Members


    [Header("Debugging")]
    [SerializeField] private ObjectType _objectType;
    [SerializeField] protected bool _isDevMode;

    protected Logger _logger;

    #endregion

    #region Unity

    protected virtual void OnEnable() { } // subscribe to events
    protected virtual void OnDisable() { } // unsubscribe to events
    protected virtual void Awake() => InitComponents();
    protected virtual void Start()
    {
        AssertComponents();
        InitVariables();
    }
    protected virtual void Update()
    {
        if (_isDevMode)            
            Test();
    }

    #endregion
    #region Helpers

    protected virtual void Test() { }

    protected virtual void AssertComponents() 
    {
        _logger.Log($"Debugging enabled using {_objectType} _logger!", _isDevMode);
    }
    protected virtual void InitComponents() 
    {
        _logger = LogManager.Instance.Loggers[(int)_objectType];
    }
    protected virtual void InitVariables() { }

    #endregion
}

public enum ObjectType
{
    ACTOR = 0,
    MANAGER = 1,
    MECHANIC = 2
}