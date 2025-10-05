using UnityEngine;
using System;

// Similar to singleton, but it OVERRIDES the new version INSTEAD OF DESTORYING it
public abstract class StaticInstance<T> : MonoBehaviour where T : MonoBehaviour
{
    #region Members

    public static T Instance { get; private set; }

    [Header("Debugging")]
    [SerializeField] protected Logger _logger;
    [SerializeField] protected bool _isDevMode;

    #endregion
    #region Methods

    protected virtual void OnEnable() { } // subscribe to events
    protected virtual void OnDisable() { } // unsubscribe to events
    protected virtual void Awake()
    {
        Instance = this as T;

        Debug.Assert(_logger, "<color=red>Missing _logger reference!</color>", gameObject);

        if (_isDevMode)
            _logger.Log($"{name}'s developer mode enabled!", this, ColorType.YELLOW);
    }
    protected virtual void OnApplicationQuit()
    {
        Instance = null;
        Destroy(gameObject);
    }

    #endregion
}

// This DESTROYS any new versions created, leaving the original alone
public abstract class Singleton<T> : StaticInstance<T> where T : MonoBehaviour 
{
    protected override void Awake() 
    {
        if (Instance != null)   
            Destroy(gameObject);
    
        base.Awake();
    }
}

// This makes the singleton SURVIVE SCENE LOADS without being destroyed
public abstract class PersistentSingleton<T> : Singleton<T> where T : MonoBehaviour
{
    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }
}