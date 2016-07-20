// File Comment
// ******************************************************
//
// File Name:               GameLocalization
//
// Tables:                  nothing
//
// Author:                  Jin Xiangkai
//
// Create Date:             2016.3.25
//
// Reference:               
//
// Revision History:
//      R1:
//          Author:         Jin Xiangkai
//          Date:           2016.4.6
//          Reason:         add font control
//******************************************************
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;
using UnityEngine;
using Object = UnityEngine.Object;

/// <summary>
/// the game's localization
/// </summary>
public class LocalizationManager
{
    #region Enumeration declaration
    /// <summary>
    /// the enumator of language
    /// </summary>
    public enum ELanguageType
    {
        Device = 0,
        English = 1,
        Chinese = 2,
        French = 3
    }
    #endregion

    #region The important dictionary
    private static Dictionary<string, string> sLocDict = new Dictionary<string, string>();
    #endregion

    #region The language and font
    public ELanguageType AppLanguage;
    private Font currFont;
    public Font CurrFont
    {
        get
        {
            return currFont;
        }
    }
    #endregion

    #region Load function
    /// <summary>
    /// load the localization text into a dictionary
    /// </summary>
    /// <param name="content">the XML config path</param>
    /// <returns></returns>
    private bool Load(string content)
    {
        try
        {
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(content);
            XmlNodeList locList = xml.LastChild.ChildNodes;
            XmlNode locNode = null;
            string locId = "";
            string info = "";
            for (int i = 0; i < locList.Count; ++i)
            {
                locNode = locList[i];
                locId = locNode.Attributes[0].Value;
                if (!sLocDict.TryGetValue(locId, out info))
                {
                    info = locNode.Attributes[1].Value;
                    sLocDict[locId] = info;
                }                
            }
        }
        catch (Exception ex)
        {
            Debug.LogError(string.Format("parse xml error {0}", ex));
            return false;
        }
        return true;
    }
    #endregion

    #region The get function
    /// <summary>
    /// get the text that you want using the LOC_KEY
    /// </summary>
    /// <param name="key">the LOC_KEY</param>
    /// <returns></returns>
    public static string Get(string key)
    {
        string val;
        key = key.TrimEnd(new char[] { '\r', '\n' });
        if (sLocDict.TryGetValue(key, out val))
        {
            return val;
        }
        //else
        //{
        //    Debug.LogWarning("GameLocalization key not found: " + key);
        //}
        return key;
    }
    #endregion

    #region Set function
    /// <summary>
    /// set the application language by device or player setting, if somthing wrong, default English
    /// </summary>
    /// <returns>the application's current language type</returns>
    public void SetApplicationLanguage(ELanguageType appLanguage = ELanguageType.Device)
    {
        this.AppLanguage = appLanguage;

        // default english
        string localizationFile = "Localization/English";

        if (AppLanguage == LocalizationManager.ELanguageType.Device)
        {
            try
            {
                switch (Application.systemLanguage)
                {
                    case SystemLanguage.English:
                        localizationFile = "Localization/English";
                        AppLanguage = LocalizationManager.ELanguageType.English;
                        break;
                    case SystemLanguage.Chinese:
                        localizationFile = "Localization/Chinese";
                        AppLanguage = LocalizationManager.ELanguageType.Chinese;

                        break;
                    case SystemLanguage.French:
                        localizationFile = "Localization/French";
                        AppLanguage = LocalizationManager.ELanguageType.French;
                        break;
                    default:
                        localizationFile = "Localization/English";
                        AppLanguage = LocalizationManager.ELanguageType.English;
                        break;
                }
            }
            catch
            {
                Debug.LogWarning("Localization config file set wrong!!! -.- Current language: English");

                localizationFile = "Localization/English";
                AppLanguage = LocalizationManager.ELanguageType.English;
            }
        }
        else
        {
            try
            {
                switch (AppLanguage)
                {
                    case LocalizationManager.ELanguageType.English:
                        localizationFile = "Localization/English";
                        break;
                    case LocalizationManager.ELanguageType.Chinese:
                        localizationFile = "Localization/Chinese";
                        break;
                    case LocalizationManager.ELanguageType.French:
                        localizationFile = "Localization/French";
                        break;
                    default:
                        localizationFile = "Localization/English";
                        AppLanguage = LocalizationManager.ELanguageType.English;
                        break;
                }
            }
            catch
            {
                Debug.LogWarning("Localization config file set wrong!!! -.- Current language: English");

                localizationFile = "Localization/English";
                AppLanguage = LocalizationManager.ELanguageType.English;
            }
        }

        string localizationContent;
        localizationContent = ResourceManager.instance.GetAssetDirectly<string>(localizationFile);
        if(localizationContent == null)
        {
            localizationContent = (Resources.Load<TextAsset>(localizationFile) as TextAsset).text;
        }
        Load(localizationContent);
    }

    /// <summary>
    /// set the application font by the language, must call after SetApplicationLanguage
    /// </summary>
    public void SetApplicationFont()
    {
        if(AppLanguage == ELanguageType.Device)
        {
            Debug.Log("LocalizationManager.cs: SetApplicationFont()--> The Font can not be set because the app language is not set!");
            return;
        }

        //string fontPath = "Font/KOMTITA";
        //switch(AppLanguage)
        //{
        //    case ELanguageType.English: fontPath = "Font/KOMTITA"; break;
        //    case ELanguageType.Chinese: fontPath = "Font/KOMTITA"; break;
        //    case ELanguageType.French: fontPath = "Font/KOMTITA"; break;
        //    default: fontPath = "Font/KOMTITA"; break;
        //}
        //Font tempFont = Resources.Load(fontPath) as Font;
        //if (tempFont)
        //    currFont = tempFont;
    }
    #endregion
}


