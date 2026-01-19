using UnityEngine;

public abstract class StaticInstance<T> : Actor where T : MonoBehaviour
{
    public static T Instance { get; private set; }

    protected override void Awake()
    {
        Instance = this as T;
        base.Awake();
    }
    protected virtual void OnApplicationQuit()
    {
        Instance = null;
        Destroy(gameObject);
    }
}

public abstract class Singleton<T> : StaticInstance<T> where T : MonoBehaviour
{
    protected override void Awake() 
    {
        if (Instance != null)   
            Destroy(gameObject);
    
        base.Awake();
    }
}

public abstract class PersistentSingleton<T> : Singleton<T> where T : StaticInstance<T>
{
    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }
}