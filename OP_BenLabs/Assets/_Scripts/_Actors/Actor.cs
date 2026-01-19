using UnityEngine;

public abstract class Actor : MonoBehaviour
{
    #region Members

    [Header("Debugging")]
    [SerializeField] private ObjectType a_objectType;
    [SerializeField] protected bool a_isDevMode;

    protected AudioManager a_audMgr;     
    protected GameManager a_gameMgr;
    protected Logger a_logger;

    #endregion

    #region Actor

    protected virtual void Test() { }
    protected virtual void AssertComponents() { }
    protected virtual void InitComponents() { }
    protected virtual void InitVariables() { }

    #endregion
    #region Unity

    protected virtual void Awake()
    {
        a_audMgr = AudioManager.Instance;
        a_gameMgr = GameManager.Instance;

        a_logger = LogManager.Instance.Loggers[(int)a_objectType];
        a_logger.Log($"Debugging enabled using <color=yellow>{a_objectType}</color> _logger!", a_isDevMode);

        AssertComponents();
        InitComponents();
    }
    protected virtual void OnEnable() { } // subscribe to events
    protected virtual void Start() => InitVariables();
    protected virtual void Update()
    {
        if (a_isDevMode)            
            Test();
    }
    protected virtual void OnDisable() { } // unsubscribe to events

    #endregion
}

public enum ObjectType
{
    Actor = 0,
    Manager = 1,
    Mechanic = 2
}
