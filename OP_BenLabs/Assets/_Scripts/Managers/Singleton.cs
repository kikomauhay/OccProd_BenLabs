using UnityEngine;

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

    protected virtual void Awake()
    {
        Instance = this as T;
        name = ToString();

        if (!_logger)
            Debug.LogWarning($"{name} is missing a Logger component!");
        
        if (_isDevMode)
            Debug.Log($"<color=yellow>{name}'s debugging is enabled!</color>");
    }
    protected virtual void OnApplicationQuit()
    {
        Instance = null;
        Destroy(gameObject);
    }

    protected virtual void OnEnable() { } // subscribe to events
    protected virtual void OnDisable() { } // unsubscribe to events
    
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