using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class AddressPopupController : MonoBehaviour
{
    [Header("UI Elements inside this Popup")]
    public TMP_InputField addressInputField;
    public TMP_InputField postalInputField;
    public TMP_Text feedbackText;

    [Header("Correct Answers")]
    [TextArea] public string correctAddress = "123 Seventh Avenue #07-67";
    public string correctPostalCode = "676767";
    public string homeSceneName = "House";

    // Called when clicking the Home option in the destination menu
    public void OpenPanel()
    {
        gameObject.SetActive(true);
        if (addressInputField != null) addressInputField.text = "";
        if (postalInputField != null) postalInputField.text = "";

        // Instantly clears "New Text" when opened
        if (feedbackText != null) feedbackText.text = "";
    }

    public void ClosePanel()
    {
        gameObject.SetActive(false);
    }

    // Hook this to your Submit/Enter button On Click event
    public void SubmitAddress()
    {
        if (addressInputField == null || postalInputField == null)
        {
            Debug.LogError("[AddressPopup] Error: Address or Postal InputField is unassigned in the Inspector!");
            return;
        }

        string playerAddress = addressInputField.text.Trim().ToLower();
        string playerPostal = postalInputField.text.Trim().ToLower();

        string targetAddress = correctAddress.Trim().ToLower();
        string targetPostal = correctPostalCode.Trim().ToLower();

        Debug.Log($"[AddressPopup] Submitting -> Entered Address: '{playerAddress}' (Target: '{targetAddress}') | Entered Postal: '{playerPostal}' (Target: '{targetPostal}')");

        if (playerAddress == targetAddress && playerPostal == targetPostal)
        {
            Debug.Log("[AddressPopup] Success! Loading home scene: " + homeSceneName);
            if (feedbackText != null) feedbackText.text = "Success!";
            ClosePanel();
            SceneManager.LoadScene(homeSceneName);
        }
        else
        {
            Debug.Log("[AddressPopup] Failed! Incorrect address or postal code entered.");
            if (feedbackText != null) feedbackText.text = "Incorrect address!";
        }
    }
}