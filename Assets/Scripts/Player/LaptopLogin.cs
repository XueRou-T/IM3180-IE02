using UnityEngine;
using TMPro;

public class LaptopLogin : MonoBehaviour
{
    public LaptopLogin loginPanel;
    public LaptopPuzzle puzzlePanel;
    public TMP_InputField passwordInput;
    public TMP_Text loginFeedbackText;

    public void TryLogin()
    {
        loginFeedbackText.text = "Incorrect password.";
    }

    public void OpenForgotPassword()
    {
        loginPanel.gameObject.SetActive(false);
        puzzlePanel.gameObject.SetActive(true);
    }

    public void CloseLogin()
    {
        loginPanel.gameObject.SetActive(false);
        passwordInput.text = "";
        loginFeedbackText.text = "";
        puzzlePanel.IsPuzzleOpen = false;
    }

    private void OnEnable()
    {
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().desktopOpen = true;
    }

    private void OnDisable()
    {
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().desktopOpen = false;
    }
}