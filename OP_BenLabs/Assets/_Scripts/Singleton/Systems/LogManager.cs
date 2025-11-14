using UnityEngine;
using UnityEngine.InputSystem.Utilities;

public class LogManager : Singleton<LogManager>
{
    public ReadOnlyArray<Logger> Loggers => _loggers;

    [Tooltip("0 = Manager, 1 = Actor, 2 = Mechanic")]
    [SerializeField] private Logger[] _loggers;

    protected override void AssertComponents()
    {
        Debug.Assert(_loggers.Length == 3, "Missing elements in _loggers", gameObject);
        base.AssertComponents();
    }
}