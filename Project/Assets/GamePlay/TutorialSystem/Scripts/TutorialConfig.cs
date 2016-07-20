using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Reflection;
using UnityEngine;

public class TutorialConfig
{
    #region Constant
    private const string filePath = "TutorialConfigs";
    #endregion

    #region Static
    public static Dictionary<int, Tutorial> sTutorialDict = new Dictionary<int, Tutorial>();

    public static bool LoadXML()
    {
        string content = ResourceManager.instance.GetAssetDirectly<string>(filePath);
        if (content == null)
        {
            TextAsset ta = Resources.Load<TextAsset>(filePath);
            if (ta != null)
            {
                return Load(ta.text);
            }
        }
        else
        {
            return Load(content);
        }

        return false;
    }

    public static bool Load(string content)
    {
        try
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(content);

            sTutorialDict.Clear();
            foreach (XmlNode childNode in doc.LastChild.ChildNodes)
            {
                if (childNode.NodeType == XmlNodeType.Comment) continue;
                int tutorialId = Convert.ToInt32(childNode.Attributes["ID"].Value);
                string triggerExpr = childNode.Attributes["Triggers"].Value;
                Tutorial t = new Tutorial(tutorialId, triggerExpr);
                foreach (XmlNode stepNode in childNode.ChildNodes)
                {
                    if (stepNode.NodeType == XmlNodeType.Comment) continue;
                    int stepId = Convert.ToInt32(stepNode.Attributes["ID"].Value);
                    TutorialStep s = new TutorialStep(stepId);
                    foreach (XmlNode actionNode in stepNode.ChildNodes)
                    {
                        if (actionNode.NodeType == XmlNodeType.Comment) continue;
                        string actionExpr = actionNode.InnerText;
                        if (string.IsNullOrEmpty(actionExpr) == false)
                        {
                            if (actionNode.Name == "OnResumeAction")
                                s.AddOnResumeAction(actionExpr);
                            else if (actionNode.Name == "Action")
                                s.AddAction(actionExpr);
                            else if (actionNode.Name == "PostStepAction")
                                s.AddPostStepAction(actionExpr);
                            else if (actionNode.Name == "NextStepCondition")
                                s.AddNextStepCondition(actionExpr);
                        }
                    }
                    t.AddTutorialStep(s);
                }
                sTutorialDict[tutorialId] = t;
            }
            return true;
        }
        catch
        {
            return false;
        }
    }
    #endregion
}

public class Tutorial
{
    public int tutorialId { get; private set; }
        
    private List<TutorialStep> steps = new List<TutorialStep>();
    private List<TutorialTrigger> triggers = new List<TutorialTrigger>();

    public Tutorial(int id, string triggerExpr)
    {
        steps.Clear();
        tutorialId = id;
        ParseTriggerExpression(triggerExpr);
    }

    private void ParseTriggerExpression(string triggerExpr)
    {
        string[] triggerList = triggerExpr.Split(new char[]{';'}, StringSplitOptions.RemoveEmptyEntries);
        foreach (string triggerStr in triggerList)
        {
            string trimmed = triggerStr.Trim();
            int leftBracketPos = trimmed.IndexOf('(');
            string triggerName = trimmed.Substring(0, leftBracketPos);
            string triggerParameters = trimmed.Substring(leftBracketPos + 1).TrimEnd(')');
            string[] triggerParameterList = triggerParameters.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            Type triggerType = Type.GetType(triggerName);
            if (triggerType != null && triggerType.IsSubclassOf(typeof(TutorialTrigger)))
            {
                TutorialTrigger t = (TutorialTrigger)(Activator.CreateInstance(triggerType));
                t.Initialize(triggerParameterList);

                triggers.Add(t);
            }
            else
            {
                throw new NotImplementedException("Failed to find class " + triggerName + " which is inherited from class TutorialTrigger. Have you implemented it in file TutorialTriggers.cs?");
            }
        }
    }

    public void AddTutorialStep(TutorialStep step)
    {
        steps.Add(step);
    }

    public bool IsTriggered(Type triggerType, object data)
    {
        TutorialTrigger trigger = triggers.FirstOrDefault((TutorialTrigger t) => t.GetType() == triggerType);
        if(trigger != null)
        {
            return trigger.IsTriggered(data);
        }
        else
        {
            return false;
        }
    }
    
    public TutorialStep GetStep(int index)
    {
        if (index >= 0 && index < steps.Count)
            return steps[index];
        else
            return null;
    }

    public int GetStepIndexById(int stepId)
    {
        int index = 0;
        foreach (TutorialStep step in steps)
        {
            if (step.stepId == stepId)
            {
                return index;
            }
            else
            {
                index++;
            }
        }
        return -1;
    }

    public TutorialStep GetStepById(int stepId)
    {
        return GetStep(GetStepIndexById(stepId));
    }
}

public class TutorialStep
{
    public int stepId { get; private set; }
    List<string> onResumeActions = new List<string>();
    List<string> actions = new List<string>();
    List<string> postStepActions = new List<string>();
    List<string> nextStepConditions = new List<string>();

    public TutorialStep(int id)
    {
        actions.Clear();
        stepId = id;
    }

    public void AddOnResumeAction(string actionExpr)
    {
        onResumeActions.Add(actionExpr);
    }

    public void AddAction(string actionExpr)
    {
        actions.Add(actionExpr);
    }

    public void AddPostStepAction(string actionExpr)
    {
        postStepActions.Add(actionExpr);
    }

    public void AddNextStepCondition(string conditionExpr)
    {
        nextStepConditions.Add(conditionExpr);
    }


    public void ExecuteOnResumeActions()
    {
        foreach (string actionExpr in onResumeActions)
        {
            ExecuteAction(actionExpr);
        }
    }

    public void ExecuteActions()
    {
        foreach (string actionExpr in actions)
        {
            ExecuteAction(actionExpr);
        }
    }

    public void ExecutePostStepActions()
    {
        foreach (string actionExpr in postStepActions)
        {
            ExecuteAction(actionExpr);
        }
    }


    private void ExecuteAction(string actionExpr)
    {
        string trimmed = actionExpr.Trim();
        int leftBracketPos = trimmed.IndexOf('(');
        string actionName = trimmed.Substring(0, leftBracketPos);
        string actionParameters = trimmed.Substring(leftBracketPos + 1).TrimEnd(')');
        string[] actionParameterList = actionParameters.Split(new char[] { ',' }, StringSplitOptions.None);
        
        MethodInfo mi = typeof(TutorialActions).GetMethod(actionName, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
        if (mi != null)
        {
            if (actionParameters != "")
                mi.Invoke(null, new object[] { actionParameterList });
            else
                mi.Invoke(null, null);
        }
        else
        {
            throw new NotImplementedException("Failed to find method: " + actionName + ", have you implemented it in class TutorialActions?");
        }
    }

    public bool CheckNextStepConditions()
    {
        foreach (string conditionExpr in nextStepConditions)
        {
            if (CheckNextStepCondition(conditionExpr) == false)
            {
                return false;
            }
        }

        return true;
    }

    private bool CheckNextStepCondition(string conditionExpr)
    {
        string trimmed = conditionExpr.Trim();
        int leftBracketPos = trimmed.IndexOf('(');
        string conditionName = trimmed.Substring(0, leftBracketPos);
        string conditionParameters = trimmed.Substring(leftBracketPos + 1).TrimEnd(')');
        string[] conditionParameterList = conditionParameters.Split(new char[] { ',' }, StringSplitOptions.None);

        MethodInfo mi = typeof(TutorialConditions).GetMethod(conditionName, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
        if (mi != null)
        {
            object result;
            if (conditionParameters != "")
                result = mi.Invoke(null, new object[] { conditionParameterList });
            else
                result = mi.Invoke(null, null);
            return (bool)result;
        }
        else
        {
            throw new NotImplementedException("Failed to find method: " + conditionName + ", have you implemented it in class TutorialConditions?");
        }
    }
}

