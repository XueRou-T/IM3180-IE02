using UnityEngine;
using TMPro;

public class LaptopPuzzle : MonoBehaviour
{
    public static bool IsPuzzleOpen = false;
    public GameObject puzzlePanel;
    public TMP_InputField answer1Input;
    public TMP_InputField answer2Input;
    public TMP_Text feedbackText;
    public GameObject desktopScreen;

    [Header("Correct Answers")]
    public string correctAnswer1 = "White Tulip";
    public string correctAnswer2 = "Nasi Lemak";

    public void SubmitAnswers()
    {
        string given1 = answer1Input.text.Trim();
        string given2 = answer2Input.text.Trim();

        bool correct1 = string.Equals(given1, correctAnswer1.Trim(), System.StringComparison.OrdinalIgnoreCase);
        bool correct2 = string.Equals(given2, correctAnswer2.Trim(), System.StringComparison.OrdinalIgnoreCase);

        if (correct1 && correct2)
        {
            feedbackText.color = Color.green;
            feedbackText.text = "";

            if (GameManager.Instance != null)
            {
                GameManager.Instance.SetFlag("LaptopSolved");
            }

            // Placeholder for later: trigger the memory-reveal video/cutscene here
            desktopScreen.SetActive(true);
            this.gameObject.SetActive(false);
        }
        else
        {
            feedbackText.color = Color.red;
            feedbackText.text = "Incorrect. Try again.";
        }
    }

    public void ClosePuzzle()
    {
        puzzlePanel.SetActive(false);
        feedbackText.text = "";
        IsPuzzleOpen = false;
    }
}