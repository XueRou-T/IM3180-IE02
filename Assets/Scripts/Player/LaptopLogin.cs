using UnityEngine;
using TMPro;

public class LaptopLogin : MonoBehaviour
{
    public GameObject loginPanel;
    public GameObject puzzlePanel;
    public TMP_InputField passwordInput;
    public TMP_Text loginFeedbackText;

    public void TryLogin()
    {
        loginFeedbackText.text = "Incorrect password.";
    }

    public void OpenForgotPassword()
    {
        loginPanel.SetActive(false);
        puzzlePanel.SetActive(true);
    }

    public void CloseLogin()
    {
        loginPanel.SetActive(false);
        passwordInput.text = "";
        loginFeedbackText.text = "";
        LaptopPuzzle.IsPuzzleOpen = false;
    }

    void OnEnable()
    {
        LaptopPuzzle.IsPuzzleOpen = true;
    }
}