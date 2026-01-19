using UnityEngine;


public class Logger : MonoBehaviour
{
    [Header("Logging Settings"), Tooltip("If ShowLogs is ticked off, the console will be clean.")]
    [SerializeField] private bool _showLogs;

    public void Assert(bool condition, string msg, Object context)
    {
        if (!condition)
        {
            Debug.Assert(condition, $"<color=red>{msg}</color>", context);
            Debug.Break();
        }
    }
    public void AssertReference(bool condition, Object context)
    {
        if (!condition)
        {
            Debug.Assert(condition, $"<color={TextColor.Red}>Missing reference/s!</color>", context);
            Debug.Break();            
        }
    }    
    public void Log(object message, bool isDevMode)
    {
        if (_showLogs && isDevMode)
            Debug.Log(message);
    }
    public void Log(object message, TextColor col, bool isDevMode)
    {
        if (_showLogs && isDevMode)
            Debug.Log($"<color={col}>{message}</color>");
    }
}

public enum TextColor
{
    Yellow = 0, // warnings
    Red = 1,    // errors
    Lime = 2, 
    Cyan = 3,
}