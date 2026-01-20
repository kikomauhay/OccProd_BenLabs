using UnityEngine;

public class LogManager : Singleton<LogManager>
{
    public Logger[] Loggers => _loggers;

    [Tooltip("0 = Actor, 1 = Manager, 2 = Mechanic")]
    [SerializeField] private Logger[] _loggers;

    protected override void AssertComponents()
    {
        a_logger.AssertReference(_loggers.Length == 
                                 System.Enum.GetValues(typeof(ObjectType)).Length, 
                                 this);
    }
}