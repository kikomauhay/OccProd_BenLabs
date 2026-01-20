using UnityEngine;

// Similar to singleton, but it OVERRIDES the new version INSTEAD OF DESTORYING it
public abstract class StaticInstance<T> : Actor where T : MonoBehaviour
{
    public static T Instance { get; private set; }

    protected override void Awake()
    {
        Instance = this as T;
        InitComponents();
    }
    protected virtual void OnApplicationQuit()
    {
        Instance = null;
        Destroy(gameObject);
    }
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