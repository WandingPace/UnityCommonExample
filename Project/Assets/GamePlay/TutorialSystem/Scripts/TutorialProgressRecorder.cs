using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class TutorialProgressRecorder
{
    private static Dictionary<int, bool> sProgressDict = new Dictionary<int,bool>();
    private static int inProgressTutorialId = -1;
    private static int inProgressStepId = -1;

    public static bool IsTutorialCompleted(int tutorialId)
    {
        return (sProgressDict.ContainsKey(tutorialId) && sProgressDict[tutorialId]);
    }

    public static void LoadProgress()
    {
        if (UserProfile.current == null) return;
        string playerUid = UserProfile.current.account.id;

        Hashtable playerTutorialProgressTable = null;
        if(PlayerPrefs.HasKey("TutorialProgress"))
        {
            string jsonStr = PlayerPrefs.GetString("TutorialProgress");
            Hashtable table = URL_JSON.JsonDecode(jsonStr) as Hashtable;
            if(table.ContainsKey(playerUid))
            {
                playerTutorialProgressTable = table[playerUid] as Hashtable;
            }
        }

        foreach(var kv in TutorialConfig.sTutorialDict)
        {
            sProgressDict[kv.Key] = false;
            if (playerTutorialProgressTable != null)
            {
                if(playerTutorialProgressTable.ContainsKey(kv.Key.ToString()))
                {
                    sProgressDict[kv.Key] = Convert.ToBoolean(playerTutorialProgressTable[kv.Key.ToString()]);
                }
            }
        }

        if (playerTutorialProgressTable != null)
        {
            if (playerTutorialProgressTable.ContainsKey("InProgressTutorial"))
            {
                inProgressTutorialId = Convert.ToInt32(playerTutorialProgressTable["InProgressTutorial"]);
            }
            if (playerTutorialProgressTable.ContainsKey("InProgressStep"))
            {
                inProgressStepId = Convert.ToInt32(playerTutorialProgressTable["InProgressStep"]);
            }
        }
    }

    public static void UpdateInProgressTutorial(int tutorialId, int stepId)
    {
        inProgressTutorialId = tutorialId;
        inProgressStepId = stepId;

        SaveProgress();
    }

    public static void UpdateProgress(int completedTutorialId)
    {
        sProgressDict[completedTutorialId] = true;
        inProgressTutorialId = -1;
        inProgressStepId = -1;

        SaveProgress();
    }

    private static void SaveProgress()
    {
        string playerUid = UserProfile.current.account.id;

        Hashtable table = null;

        if (PlayerPrefs.HasKey("TutorialProgress"))
        {
            string jsonStr = PlayerPrefs.GetString("TutorialProgress");
            table = URL_JSON.JsonDecode(jsonStr) as Hashtable;
        }
        else
        {
            table = new Hashtable();
        }

        if (table.ContainsKey(playerUid))
        {
            (table[playerUid] as Hashtable)["InProgressTutorial"] = inProgressTutorialId;
            (table[playerUid] as Hashtable)["InProgressStep"] = inProgressStepId; 
            foreach (var kv in sProgressDict)
            {
                (table[playerUid] as Hashtable)[kv.Key.ToString()] = kv.Value;
            }
        }
        else
        {
            table[playerUid] = new Hashtable();
            (table[playerUid] as Hashtable)["InProgressTutorial"] = inProgressTutorialId;
            (table[playerUid] as Hashtable)["InProgressStep"] = inProgressStepId;
            foreach (var kv in sProgressDict)
            {
                (table[playerUid] as Hashtable)[kv.Key.ToString()] = kv.Value;
            }
        }

        string result = URL_JSON.JsonEncode(table);
        PlayerPrefs.SetString("TutorialProgress", result);
        PlayerPrefs.Save();
    }
    
#if UNITY_EDITOR
    [MenuItem("Debug/Tutorial/ClearProgress")]
#endif
    public static void ClearProgress()
    {
        sProgressDict.Clear();
        string playerUid = UserProfile.current.account.id;
        if (PlayerPrefs.HasKey("TutorialProgress"))
        {
            string jsonStr = PlayerPrefs.GetString("TutorialProgress");
            Hashtable table = URL_JSON.JsonDecode(jsonStr) as Hashtable;

            if(table.ContainsKey(playerUid))
            {
                table.Remove(playerUid);

                string result = URL_JSON.JsonEncode(table);
                PlayerPrefs.SetString("TutorialProgress", result);
                PlayerPrefs.Save();
            }
        }
    }
}
