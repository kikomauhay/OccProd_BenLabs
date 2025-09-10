using UnityEngine;

public abstract class Actor : MonoBehaviour
{
    #region SerializeField

    [Header("Debgging")]
    [SerializeField] protected Logger _logger;
    [SerializeField] protected bool _isDevMode;

    #endregion

    #region Unity

    protected virtual void OnEnable() { }
    protected virtual void OnDisable() { }
    protected virtual void Start()
    {
        InitComponents();
        InitVariables();

        if (_isDevMode)
            Debug.Log("<color=yellow>Debugging is enabled!</color>", this);
    }

    #endregion
    #region Helpers

    protected virtual void InitComponents() { }
    protected virtual void InitVariables() { }

    #endregion
}
