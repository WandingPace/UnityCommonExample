using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public class TutorialActions
{
    #region SkipCurrentStep
    // DO: Skip the current step and try to goto the next step.
    // PARAMS: No parameters.
    // REMARKS: The tutorial will still have to wait until all next step conditions are passed.

    public static void SkipCurrentStep()
    {
        TutorialManager.instance.SkipCurrentStep();
    }
    #endregion

    #region SkipToStep
    // DO: Skip the tutorial directly to specified step.
    // PARAMS: [0] - step id.

    public static void SkipToStep(string[] parameters)
    {
        if (parameters.Length != 1) return;
        TutorialManager.instance.SkipToStep(Int32.Parse(parameters[0]));
    }
    #endregion

    #region SaveAsCheckpoint
    // DO: Save the current tutorial and step as the checkpoint.
    // PARAMS: No parameters.

    public static void SaveAsCheckpoint()
    {
        TutorialManager.instance.SaveCurrentProgress();
    }
    #endregion

    #region SaveCheckpoint
    // DO: Save the specified tutorial and step as the checkpoint.
    // PARAMS: [0] - tutorial id, [1] - step id.

    public static void SaveCheckpoint(string[] parameters)
    {
        if (parameters.Length != 2) return;
        TutorialProgressRecorder.UpdateInProgressTutorial(Convert.ToInt32(parameters[0]), Convert.ToInt32(parameters[1]));
    }
    #endregion

    #region SetActive
    // DO: Set the GameObject of the specified path to active/inactive.
    // PARAMS: [0] - the hierarchy path of the GameObject, [1] - true/false.

    public static void SetActive(string[] parameters)
    {
        if (parameters.Length != 2) return;

        GameObject obj = GameObject.Find(parameters[0]);
        if(obj != null)
        {
            obj.SetActive(bool.Parse(parameters[1]));
        }
    }
    #endregion

    #region SetLocalPosition
    // DO: Set the local position of GameObject of the specified path.
    // PARAMS: [0] - the hierarchy path of the GameObject, [1] - x, [2] - y, [3] - z.

    public static void SetLocalPosition(string[] parameters)
    {
        if (parameters.Length != 4) return;

        GameObject obj = GameObject.Find(parameters[0]);
        if (obj != null)
        {
            obj.transform.localPosition = new Vector3
            (
                Single.Parse(parameters[1]),
                Single.Parse(parameters[2]),
                Single.Parse(parameters[3])
            );
        }
    }
    #endregion

    #region SetPosition
    // DO: Set the world position of GameObject of the specified path.
    // PARAMS: [0] - the hierarchy path of the GameObject, [1] - x, [2] - y, [3] - z.

    public static void SetPosition(string[] parameters)
    {
        if (parameters.Length != 4) return;

        GameObject obj = GameObject.Find(parameters[0]);
        if (obj != null)
        {
            obj.transform.position = new Vector3
            (
                Single.Parse(parameters[1]),
                Single.Parse(parameters[2]),
                Single.Parse(parameters[3])
            );
        }
    }
    #endregion

    #region SetValue
    // DO: Set the value of specified field/property in specified component of the specified GameObject.
    // PARAMS: [0] - the hierarchy path of the GameObject, [1] - component name, [2] - field/property name, [3] - value.
    public static void SetValue(string[] parameters)
    {
        if (parameters.Length < 4) return;

        GameObject gameObj = GameObject.Find(parameters[0]);
        if (gameObj != null)
        {
            Component c = gameObj.GetComponent(parameters[1]);
            if (c == null) 
            {
                Debug.LogError("Failed to find component " + parameters[1] + " in " + parameters[0] + ".");
                return; 
            }

            string[] fieldSections = parameters[2].Split('.');

            object obj = c;
            for (int i = 0; i < fieldSections.Length; ++i)
            {
                string fieldSection = fieldSections[i];
                if(i == fieldSections.Length - 1)
                {
                    if (obj != null)
                    {

                    }
                }
                else
                {
                    FieldInfo fi = obj.GetType().GetField(fieldSection, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);
                    if (fi != null)
                    {
                        obj = fi.GetValue(obj);
                    }
                    else
                    {
                        PropertyInfo pi = obj.GetType().GetProperty(parameters[2], BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);
                        if (pi != null)
                        {
                            obj = pi.GetGetMethod().Invoke(obj, new object[] { Convert.ChangeType(parameters[3], pi.DeclaringType) });
                        }
                        else
                        {
                            Debug.LogError("Failed to find " + fieldSection + ".");
                            return;
                        }
                    }
                }
            }
        }
        else
        {
            Debug.LogError("Failed to find GameObject " + parameters[0] + ".");
        }
    }
    #endregion

    #region ShowMask
    // DO: Show the semi-transparent black mask.
    // PARAMS: No parameters.

    public static void ShowMask()
    {
        TutorialManager.instance.ShowMask();
    }
    #endregion

    #region HideMask
    // DO: Hide the semi-transparent black mask.
    // PARAMS: No parameters.

    public static void HideMask()
    {
        TutorialManager.instance.HideMask();
    }
    #endregion

    #region SetupMask
    // DO: Setup the tutorial mask.
    // PARAMS: [0] - x, [1] - y, [2] - width, [3] - height, [4] - alpha (optional).
    public static void SetupMask(string[] parameters)
    {
        if(parameters.Length == 4)
        {
            TutorialManager.instance.SetupMask
            (
                Convert.ToSingle(parameters[0]),
                Convert.ToSingle(parameters[1]),
                Convert.ToSingle(parameters[2]),
                Convert.ToSingle(parameters[3])
            );
        }
        else if (parameters.Length == 5)
        {
            TutorialManager.instance.SetupMask
            (
                Convert.ToSingle(parameters[0]),
                Convert.ToSingle(parameters[1]),
                Convert.ToSingle(parameters[2]),
                Convert.ToSingle(parameters[3]),
                Convert.ToSingle(parameters[4])
            );
        }
    }
    #endregion

    #region SetForeground
    // DO: Setup the foreground sprite.
    // PARAMS: [0] - Sprite path, [1] - x, [2] - y, [3] - width, [4] - height.

    public static void SetForeground(string[] parameters)
    {
        if (parameters.Length != 5) return;
        string sprite = parameters[0];
        Vector4 rect = new Vector4
        (
            Convert.ToSingle(parameters[1]),
            Convert.ToSingle(parameters[2]),
            Convert.ToSingle(parameters[3]),
            Convert.ToSingle(parameters[4])
        );
        TutorialManager.instance.SetForeground(sprite);
        TutorialManager.instance.SetForegroundRect(rect);
    }
    #endregion

    #region SetContent
    // DO: Setup the content.
    // PARAMS: [0] - LocID of the content, [1] - x, [2] - y, [3] - width, [4] - height.
    // REMARKS: If the LocID doesn't exist, the content of the LocID string will be displayed directly.

    public static void SetContent(string[] parameters)
    {
        if (parameters.Length != 5) return;
        string content = LocalizationManager.Get(parameters[0]);
        Vector4 rect = new Vector4
        (
            Convert.ToSingle(parameters[1]),
            Convert.ToSingle(parameters[2]),
            Convert.ToSingle(parameters[3]),
            Convert.ToSingle(parameters[4])
        );
        TutorialManager.instance.SetContent(content);
        TutorialManager.instance.SetContentRect(rect);
    }
    #endregion

}
