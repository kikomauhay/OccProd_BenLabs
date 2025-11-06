using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// 
/// HOW TO USE:
///     - Attach to scripts so you can toggle them one by one.
///     - Attach it to a main script and have GOs reference it.
///     - You can add color manually by typing "<color=ColorType>message</color>"
/// 
/// </summary>

public class Logger : MonoBehaviour
{
    [Header("Logging Settings"), Tooltip("If ShowLogs is ticked off, the console will be clean.")]
    [SerializeField] private bool _showLogs;

    public void Log(object message, bool isDevMode)
    {
        if (_showLogs && isDevMode)
            Debug.Log(message);
    }
    public void Log(object message, Object sender, bool isDevMode)
    {
        if (_showLogs && isDevMode)
            Debug.Log(message, sender);
    }
    public void Log(object message, TextColor col, bool isDevMode)
    {
        if (_showLogs)
            Debug.Log($"<color={col}>{message}</color>");
    }
    public void Log(object message, Object sender, TextColor col, bool isDevMode)
    {
        if (_showLogs && isDevMode)
            Debug.Log($"<color={col}>{message}</color>", sender);
    }
}

public enum TextColor
{
    SILVER = 0, // default console color
    YELLOW = 1, // warnings
    RED = 2,    // errors
    GREEN = 3, 
    CYAN = 4,
    PINK = 5       
}