using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class TutorialConditions
{
    // Check if the current screen is one of the specified screen types.
    // PARAMETERS: [0~n] - string of the screen types.
    public static bool WaitForScreen(string[] parameters)
    {
        return parameters.Contains(UIContextManager.instance.TopScreenType.ToString());
    }
}
