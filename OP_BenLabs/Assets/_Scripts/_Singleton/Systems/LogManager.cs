using UnityEngine;

public class LogManager : Singleton<LogManager>
{
    public Logger[] Loggers => _loggers;

    [Tooltip("0 = Actor, 1 = Manager, 2 = Component")]
    [SerializeField] private Logger[] _loggers;

    protected override void AssertReferences()
    {
        a_logger.AssertReference(_loggers.Length == 3, gameObject);
    }
}