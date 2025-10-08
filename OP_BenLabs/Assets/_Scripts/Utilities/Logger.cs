using UnityEngine;

/// <summary>
/// 
/// Designed by: Isagani Coel Factora
/// 
/// HOW TO USE:
///     - Attach to scripts so you can toggle them one by one.
///     - Attach it to a main script and have GOs reference it.
///     - You can add color by doing "<color=ColorType>message</color>"
/// 
/// </summary>

public class Logger : MonoBehaviour
{
    [Header("Logging Settings"), Tooltip("If ShowLogs is ticked off, the console will be clean.")]
    [SerializeField] private bool _showLogs;

    public void Log(object message)
    {
        if (_showLogs)
            Debug.Log(message);
    }
    public void Log(object message, Object sender)
    {
        if (_showLogs)
            Debug.Log(message, sender);
    }
    public void Log(object message, ColorType col)
    {
        if (_showLogs)
            Debug.Log($"<color={col}>{message}</color>");
    }
    public void Log(object message, Object sender, ColorType col)
    {
        if (_showLogs)
            Debug.Log($"<color={col}>{message}</color>", sender);
    }
}

public enum ColorType
{
    SILVER = 0, // default console color
    YELLOW = 1, // warnings
    RED = 2,    // errors
    GREEN = 3, 
    CYAN = 4,
    PINK = 5       
}