using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class DebugDisplay : MonoBehaviour
{
    #region Members
        
    private Dictionary<string, string> _debugLogs = new Dictionary<string, string>();
    public TextMeshProUGUI _display;

    #endregion

    #region Members
        
    private void OnEnable() => Application.logMessageReceived += HandleLog;
    private void OnDisable() => Application.logMessageReceived -= HandleLog;

    private void HandleLog(string logString, string stackTrace, LogType type)
    {
        if (type == LogType.Log)
        {
            string[] splitString = logString.Split(char.Parse(":"));
            string debugKey = splitString[0];
            string debugValue = splitString.Length > 1 ? splitString[1] : " ";

            if (_debugLogs.ContainsKey(debugKey))
                _debugLogs[debugKey] = debugValue;
        
            else 
                _debugLogs.Add(debugKey, debugValue);
        }

        string displayText = " ";

        foreach (KeyValuePair<string, string> log in _debugLogs)
        {
            if (log.Value == " ")
                displayText += log.Key + "\n";
            
            else
                displayText += log.Key + ": " + log.Value + "\n";
            
            _display.text = displayText;
        }
    }

    #endregion 
}
