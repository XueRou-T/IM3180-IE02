using UnityEngine;
using UnityEngine.SceneManagement;

public class DestinationChoice : MonoBehaviour
{
    public static bool IsChoiceOpen = false;

    public CanvasGroup choicePanelGroup;
    public GameObject labButton;
    public GameObject houseButton;
    public GameObject hawkerButton;

    void Start()
    {
        SetPanelVisible(false);
    }

    public void OpenChoice()
    {
        SetPanelVisible(true);
        IsChoiceOpen = true;

        string currentScene = SceneManager.GetActiveScene().name;
        labButton.SetActive(currentScene != "Lab");
        houseButton.SetActive(currentScene != "House");
        hawkerButton.SetActive(currentScene != "HawkerCentre");
    }

    public void GoToLab() { LoadDestination("Lab"); }

    // Replaced GoToHouse to trigger the address prompt via SceneDoor
    public void SelectHomeViaDoor()
    {
        SetPanelVisible(false);
        IsChoiceOpen = false;

        // Updated to the new Unity method
        SceneDoor activeDoor = FindAnyObjectByType<SceneDoor>();
        if (activeDoor != null)
        {
            activeDoor.SelectHomeOption();
        }
    }

    public void GoToHawker() { LoadDestination("HawkerCentre"); }

    void LoadDestination(string sceneName)
    {
        PlayerPersistence.nextSpawnPointName = "SpawnPoint";
        SetPanelVisible(false);
        IsChoiceOpen = false;
        SceneManager.LoadScene(sceneName);
    }

    public void CancelChoice()
    {
        SetPanelVisible(false);
        IsChoiceOpen = false;
    }

    void SetPanelVisible(bool visible)
    {
        choicePanelGroup.alpha = visible ? 1f : 0f;
        choicePanelGroup.interactable = visible;
        choicePanelGroup.blocksRaycasts = visible;
    }
}