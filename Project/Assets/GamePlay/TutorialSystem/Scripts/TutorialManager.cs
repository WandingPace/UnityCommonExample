using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TutorialManager : MonoSingleton<TutorialManager>, IPointerClickHandler
{
    #region Constants
    private const float TUTORIAL_FADE_TIME = 0.4f;
    #endregion

    #region State
    public bool IsInTutorial { get { return currentTutorial != null; } }
    public bool isFadeFinished = false;
    private Tutorial currentTutorial = null;
    private int currentStepIndex = 0;
    private TutorialStep currentStep = null;
    private bool intendToGotoNextStep = false;
    #endregion

    #region UI
    [SerializeField]
    private CanvasGroup tutorialFadeMask;
    [SerializeField]
    private CanvasGroup tutorialCG;
    [SerializeField]
    private TutorialMask tutorialMask;
    [SerializeField]
    private Image imgForeground;
    [SerializeField]
    private Text txtContent;

    [SerializeField]
    private SpriteSet tutorialSpriteSet;
    #endregion

    #region MonoBehaviour
    void Start()
    {
        TutorialConfig.LoadXML();
        TutorialProgressRecorder.LoadProgress();

        OnGameStart();
    }
    #endregion

    #region Main Logic
    /// <summary>
    /// Called when the game starts.
    /// Check if any tutorial was in progress. Resume it if exists.
    /// </summary>
    public void OnGameStart()
    {
        // Check if there's any previous tutorial in progress.

        OnTrigger(typeof(GameStartTutorialTrigger), null);
    }

    /// <summary>
    /// Called whenever some trigger might be triggered.
    /// e.g. Starting to play a PVE level.
    ///     triggerType is: typeof(PVELevelTutorialTrigger),
    ///     data is: int PVELevelID.
    /// </summary>
    /// <param name="triggerType">Type of trigger.</param>
    /// <param name="data">Data of the trigger value.</param>
    public void OnTrigger(Type triggerType, object data)
    {
        foreach (var kv in TutorialConfig.sTutorialDict)
        {
            if (kv.Value.IsTriggered(triggerType, data) && TutorialProgressRecorder.IsTutorialCompleted(kv.Key) == false)
            {
                if(kv.Value == currentTutorial)
                {
                    // This tutorial is already triggered and is running.
                    return;
                }
                
                currentTutorial = kv.Value;
                currentStepIndex = 0;
                currentStep = currentTutorial.GetStep(0);
                intendToGotoNextStep = false;
                
                isFadeFinished = false;
                tutorialFadeMask.alpha = 0f;
                tutorialFadeMask.gameObject.SetActive(true);
                LeanTween.alphaCanvas(tutorialFadeMask, 1f, TUTORIAL_FADE_TIME).setOnComplete(() =>
                {
                    currentStep.ExecuteActions();
                    tutorialCG.alpha = 1f;
                    tutorialCG.interactable = true;
                    tutorialCG.blocksRaycasts = true;

                    isFadeFinished = true;
                    LeanTween.alphaCanvas(tutorialFadeMask, 0f, TUTORIAL_FADE_TIME).setOnComplete(()=>
                    {
                        tutorialFadeMask.gameObject.SetActive(false);
                        isFadeFinished = false;
                    });
                });
                    
                break;
            }
        }
    }

    void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
    {
        if (tutorialFadeMask.gameObject.activeSelf == false)
        {
            intendToGotoNextStep = true;
        }
    }

    /// <summary>
    /// Called when player click the UI to try getting to next step.
    /// </summary>
    public void OnGotoNextStep()
    {
        if(currentTutorial != null)
        {
            if (currentStep.CheckNextStepConditions() == false)
                return;

            if (currentTutorial.GetStep(currentStepIndex + 1) != null)
            {
                currentStepIndex++;
                currentStep = currentTutorial.GetStep(currentStepIndex);
                currentStep.ExecuteActions();
            }
            else
            {
                // A tutorial ends here.
                TutorialProgressRecorder.UpdateProgress(currentTutorial.tutorialId);

                isFadeFinished = false;
                tutorialFadeMask.alpha = 0f;
                tutorialFadeMask.gameObject.SetActive(true);
                LeanTween.alphaCanvas(tutorialFadeMask, 1f, TUTORIAL_FADE_TIME).setOnComplete(() =>
                {
                    currentTutorial = null;
                    currentStepIndex = 0;
                    currentStep = null;
                    tutorialCG.alpha = 0f; 
                    tutorialCG.blocksRaycasts = false;
                    tutorialCG.interactable = false;
                    
                    isFadeFinished = true;
                    LeanTween.alphaCanvas(tutorialFadeMask, 0f, TUTORIAL_FADE_TIME).setOnComplete(()=>
                    {
                        tutorialFadeMask.gameObject.SetActive(false);
                        isFadeFinished = false;
                    });
                });
            }
        }
        else
        {
            // We are not in a tutorial so why you try to go to next step?
        }
    }
    #endregion

    #region Implementation for TutorialActions
    public void SkipCurrentStep()
    {
        intendToGotoNextStep = true;
    }

    public void SkipToStep(int stepId)
    {
        currentStepIndex = currentTutorial.GetStepIndexById(stepId);
        currentStep = currentTutorial.GetStep(currentStepIndex);
        intendToGotoNextStep = false;

        isFadeFinished = false;
        tutorialFadeMask.alpha = 0f;
        tutorialFadeMask.gameObject.SetActive(true);
        LeanTween.alphaCanvas(tutorialFadeMask, 1f, TUTORIAL_FADE_TIME).setOnComplete(() =>
        {
            currentStep.ExecuteActions();
            tutorialCG.alpha = 1f;
            tutorialCG.interactable = true;
            tutorialCG.blocksRaycasts = true;

            isFadeFinished = true;
            LeanTween.alphaCanvas(tutorialFadeMask, 0f, TUTORIAL_FADE_TIME).setOnComplete(() =>
            {
                tutorialFadeMask.gameObject.SetActive(false);
                isFadeFinished = false;
            });
        });
    }

    public void SaveCurrentProgress()
    {
        if (currentTutorial != null)
            TutorialProgressRecorder.UpdateInProgressTutorial(currentTutorial.tutorialId, currentStep.stepId);
        else
            TutorialProgressRecorder.UpdateInProgressTutorial(-1, -1);
    }

    public void ShowMask()
    {
        tutorialMask.BlockRaycast = true;
        tutorialMask.Alpha = 1f;
    }

    public void HideMask()
    {
        tutorialMask.BlockRaycast = false;
        tutorialMask.Alpha = 0f;
    }

    public void SetupMask(float x, float y, float w, float h, float a = 1f)
    {
        tutorialMask.Location = new Vector4(x, y, w, h);
        tutorialMask.Alpha = a;
    }

    public void SetForeground(string spriteName)
    {
        if (spriteName == "" || tutorialSpriteSet.Exists(spriteName) == false)
        {
            imgForeground.gameObject.SetActive(false);
            imgForeground.sprite = null;
        }
        else
        {
            imgForeground.gameObject.SetActive(true);
            Sprite sprite = null;
            tutorialSpriteSet.TryGet(spriteName, out sprite);
            imgForeground.sprite = sprite;
        }
    }

    public void SetForegroundRect(Vector4 rect)
    {
        imgForeground.rectTransform.anchoredPosition = new Vector2(rect.x, rect.y);
        imgForeground.rectTransform.sizeDelta = new Vector2(rect.z, rect.w);
    }

    public void SetContent(string content)
    {
        txtContent.text = content;
    }

    public void SetContentRect(Vector4 rect)
    {
        txtContent.rectTransform.anchoredPosition = new Vector2(rect.x, rect.y);
        txtContent.rectTransform.sizeDelta = new Vector2(rect.z, rect.w);
    }
    #endregion

    #region MonoBehaviour interfaces
    void Update()
    {
        // If the player intends to go to next step, we will check if it's possible each frame.
        if (intendToGotoNextStep == true && currentStep != null)
        {
            OnGotoNextStep();
        }
    }
    #endregion
}
