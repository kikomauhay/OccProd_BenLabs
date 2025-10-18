using UnityEngine;

public abstract class Actor : MonoBehaviour
{
    #region Members

    [Header("Debugging")]
    [SerializeField] protected Logger _logger;
    [SerializeField] protected bool _isDevMode;

    #endregion

    #region Unity

    protected virtual void OnEnable() { } // subscribe to events
    protected virtual void OnDisable() { } // unsubscribe to events
    protected virtual void Awake() => InitComponents();
    protected virtual void Start()
    {
        Debug.Assert(_logger, "<color=red>Missing _logger reference!</color>", gameObject);

        if (_isDevMode)
            _logger.Log($"{name}'s developer mode is enabled!", gameObject, TextColor.YELLOW);

        InitVariables();
    }
    protected virtual void Update()
    {
        if (_isDevMode)            
            Test();
    }

    #endregion
    #region Helpers

    protected virtual void InitComponents() { }
    protected virtual void InitVariables() { }
    protected virtual void Test() { }

    #endregion
}
