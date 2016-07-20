
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// change the uGUI's text to use localization
/// </summary>
public class LocalText : Text
{
    private string locKeyText;
    [SerializeField]
    private bool updateText = true;

    protected override void Awake()
    {
        base.Awake();
        if (Application.isPlaying)
        {
            locKeyText = this.text;
        }
    }

    protected override void Start()
    {
        base.Start();
        if (Application.isPlaying)
        {
            locKeyText = this.text;
            if (updateText)
            {
                this.text = LocalizationManager.Get(locKeyText);
            }
            if (GameManager.instance.localizationManager.CurrFont)
            {
                this.font = GameManager.instance.localizationManager.CurrFont;
            }
        }
    }
}
