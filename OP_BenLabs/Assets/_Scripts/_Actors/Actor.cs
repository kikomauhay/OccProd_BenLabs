using UnityEngine;

public abstract class Actor : MonoBehaviour
{
    #region Members

    [Header("Debugging")]
    [SerializeField] private LoggerType a_loggerType;
    [SerializeField] protected bool a_isDevMode;

    protected AudioManager a_audMgr;
    protected GameManager a_gameMgr;
    protected Logger a_logger;

    #endregion

    #region Actor

    protected virtual void Test() { }
    protected virtual void InitComponents() { }
    protected virtual void AssertReferences() { }
    protected virtual void InitVariables() { }

    #endregion
    #region Unity

    protected virtual void Awake() => InitComponents();
    protected virtual void OnEnable() { } // subscribe to events
    protected virtual void Start()
    {
        a_audMgr = AudioManager.Instance;
        a_gameMgr = GameManager.Instance;

        a_logger = LogManager.Instance.Loggers[(int)a_loggerType];
        a_logger.Log($"Activated Logger with type: <color=yellow>{a_loggerType}</color>", a_isDevMode); 

        AssertReferences();
        InitVariables();
    }
    protected virtual void Update()
    {
        if (a_isDevMode)            
            Test();
    }
    protected virtual void OnDisable() { } // unsubscribe to events

    #endregion
}