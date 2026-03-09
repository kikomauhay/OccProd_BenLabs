using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Logger : MonoBehaviour
{
    [Header("Logging Settings"), Tooltip("If ShowLogs is ticked off, the console will be clean.")]
    [SerializeField] private bool _showLogs;

    public void Assert(bool condition, string msg, Object context)
    {
        if (!condition)
            Debug.Assert(condition, $"<color=red>{msg}</color>", context);
    }
    public void AssertReference(bool condition, Object context)
    {
        if (!condition)
            Assert(condition, $"<color={TextColor.Red}>Missing {context}!</color>", context);
    }    
    public void AssertCollection<T>(IEnumerable<T> collection, Object context)
    {
        if (collection.Count() == 0)
        {
            Assert(collection.Count() != 0, $"Walang laman yung {collection}", context);    
            return;
        }

        foreach (T item in collection)
            AssertReference(item != null, context);
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

public enum LoggerType
{
    Actor = 0,
    Manager = 1,
    Component = 2
}
