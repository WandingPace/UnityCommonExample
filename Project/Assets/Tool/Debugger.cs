using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Debugger : MonoSingleton<Debugger>
{
    private static bool showDebug = false;
    public static bool ShowDebug { get { return showDebug; } }
    

    private List<string> messages = new List<string>();
    private int maxLines = 20;

    public static void Log(string message) 
    {
        if (string.IsNullOrEmpty(message))
            return;

        System.DateTime now = System.DateTime.Now;
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        sb.Append(now.Year.ToString()).Append("/")
            .Append(now.Month.ToString("D2")).Append("/")
            .Append(now.Day.ToString("D2")).Append(" ")
            .Append(now.Hour.ToString("D2")).Append(":")
            .Append(now.Minute.ToString("D2")).Append(":")
            .Append(now.Second.ToString("D2")).Append(".")
            .Append(now.Millisecond.ToString("D3")).Append(": ");

        Debugger.instance.Add(sb.ToString() + message);
    }

    private void Add(string message) 
    {
        messages.Add(message);
    }

    private void OnGUI() 
    {
        Color c = GUI.color;
        GUI.color = Color.red;
        showDebug = GUI.Toggle(new Rect(Screen.width - 100f, 0f, 100f, 100f), showDebug, "DBG", "button");
        GUI.color = c;
        return; // We won't display these messages at all in any situation.

        int lineStart = Mathf.Max(0, messages.Count - maxLines);
        for (int i = lineStart; i < messages.Count; ++i)
        {
            GUILayout.Label(messages[i]);
        }

        if (showDebug)
        {
            
        }
    }
}
