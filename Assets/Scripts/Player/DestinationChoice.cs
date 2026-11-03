using UnityEngine;
using UnityEngine.SceneManagement;

public class DestinationChoice : MonoBehaviour
{
    public static bool IsChoiceOpen = false;

    public CanvasGroup choicePanelGroup;
    public GameObject labButton;
    public GameObject houseButton;
    public GameObject hawkerButton;

    [HideInInspector] public SceneDoor currentActiveDoor;

    void Awake()
    {
        // Force reset static flags and hide the panel safely on boot
        IsChoiceOpen = false;
        currentActiveDoor = null;
        SetPanelVisible(false);
    }

    void Start()
    {
        SetPanelVisible(false);
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SetPanelVisible(false);
        IsChoiceOpen = false;
        currentActiveDoor = null;
    }

    public void OpenChoice(SceneDoor doorCallingThis)
    {
        currentActiveDoor = doorCallingThis;

        // Ensure the parent GameObject is active in case it was disabled elsewhere
        if (gameObject != null && !gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }

        SetPanelVisible(true);
        IsChoiceOpen = true;

        string currentScene = SceneManager.GetActiveScene().name;
        if (labButton != null) labButton.SetActive(currentScene != "Lab");
        if (houseButton != null) houseButton.SetActive(currentScene != "House");
        if (hawkerButton != null) hawkerButton.SetActive(currentScene != "HawkerCentre");
    }

    public void GoToLab() { LoadDestination("Lab"); }

    public void SelectHomeViaDoor()
    {
        SetPanelVisible(false);
        IsChoiceOpen = false;

        if (currentActiveDoor != null)
        {
            currentActiveDoor.SelectHomeOption();
        }
        else
        {
            SceneDoor fallbackDoor = FindAnyObjectByType<SceneDoor>();
            if (fallbackDoor != null) fallbackDoor.SelectHomeOption();
        }
    }

    public void GoToHawker() { LoadDestination("HawkerCentre"); }

    void LoadDestination(string sceneName)
    {
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
        if (choicePanelGroup != null)
        {
            choicePanelGroup.alpha = visible ? 1f : 0f;
            choicePanelGroup.interactable = visible;
            choicePanelGroup.blocksRaycasts = visible;
        }
    }
}