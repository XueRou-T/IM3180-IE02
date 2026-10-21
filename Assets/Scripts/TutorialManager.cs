using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    public enum TargetType
    {
        None,
        UI,
        World
    }

    public enum AdvanceCondition
    {
        ContinueButton,
        PlayerMovesRight,
        MoveSlotsFilled,
        ButtonActivated
    }

    [Serializable]
    public class TutorialStep
    {
        [TextArea(2, 5)]
        public string instruction;

        public TargetType targetType;
        public RectTransform uiTarget;
        public Transform worldTarget;

        public AdvanceCondition advanceCondition;
    }

    [Header("References")]
    public TutorialPanelManager spotlight;
    public TMP_Text instructionText;
    public Button continueButton;

    [Header("Tutorial Steps")]
    public List<TutorialStep> steps = new List<TutorialStep>();

    private int currentStepIndex;
    private bool tutorialFinished;

    private void Start()
    {
        if (continueButton != null)
            continueButton.onClick.AddListener(OnContinueClicked);

        if (steps.Count > 0)
            ShowStep(0);
        else
            FinishTutorial();
    }

    private void OnDestroy()
    {
        if (continueButton != null)
            continueButton.onClick.RemoveListener(OnContinueClicked);
    }


    private void ShowStep(int index)
    {
        if (index >= steps.Count)
        {
            FinishTutorial();
            return;
        }

        currentStepIndex = index;

        TutorialStep step = steps[index];

        if (instructionText != null)
            instructionText.text = step.instruction;

        bool needsContinue = step.advanceCondition == AdvanceCondition.ContinueButton;

        if (continueButton != null)
            continueButton.gameObject.SetActive(needsContinue);

        Time.timeScale = needsContinue ? 0f : 1f;

        if (spotlight != null)
        {
            switch (step.targetType)
            {
                case TargetType.UI:
                    if (step.uiTarget != null)
                        spotlight.ShowUITarget(step.uiTarget);
                    break;

                case TargetType.World:
                    if (step.worldTarget != null)
                        spotlight.ShowWorldTarget(step.worldTarget);
                    break;

                case TargetType.None:
                    spotlight.gameObject.SetActive(false);
                    break;
            }
        }
    }


    private void AdvanceStep()
    {
        if (tutorialFinished)
            return;

        ShowStep(currentStepIndex + 1);
    }

    public void OnPlayerMovedRight()
    {
        if (IsCurrentCondition(AdvanceCondition.PlayerMovesRight))
            AdvanceStep();
    }

    public void OnMoveSlotsFilled()
    {
        if (IsCurrentCondition(AdvanceCondition.MoveSlotsFilled))
            AdvanceStep();
    }

    public void OnButtonActivated()
    {
        if (IsCurrentCondition(AdvanceCondition.ButtonActivated))
            AdvanceStep();
    }

    public void OnContinueClicked()
    {
        if (IsCurrentCondition(AdvanceCondition.ContinueButton))
        {
            Time.timeScale = 1f;
            AdvanceStep();
        }
    }

    private bool IsCurrentCondition(AdvanceCondition condition)
    {
        return !tutorialFinished
            && currentStepIndex >= 0
            && currentStepIndex < steps.Count
            && steps[currentStepIndex].advanceCondition == condition;
    }

    private void FinishTutorial()
    {
        tutorialFinished = true;

        if (spotlight != null)
            spotlight.gameObject.SetActive(false);

        if (instructionText != null)
            instructionText.gameObject.SetActive(false);

        if (continueButton != null)
            continueButton.gameObject.SetActive(false);
    }
}
