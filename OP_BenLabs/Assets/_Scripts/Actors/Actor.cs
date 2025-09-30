using UnityEngine;

public abstract class Actor : MonoBehaviour
{
    #region Members

    [Header("Debugging")]
    [SerializeField] protected Logger _logger;
    [SerializeField] protected bool _isDevMode;

    #endregion

    #region Methods

    protected virtual void OnEnable() { } // subscribe to events
    protected virtual void OnDisable() { } // unsubscribe to events
    protected virtual void Start()
    {
        InitComponents();
        InitVariables();

        Debug.Assert(_logger, "<color=red>Missing _logger reference!</color>", gameObject);

        if (_isDevMode)
            Debug.Log($"<color=yellow>{name}'s debugging is enabled!</color>");
    }

    #endregion
    #region Helpers

    protected virtual void InitComponents() { }
    protected virtual void InitVariables() { }

    #endregion
}
