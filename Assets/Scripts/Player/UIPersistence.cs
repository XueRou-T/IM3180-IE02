using UnityEngine;

public class UIPersistence : MonoBehaviour
{
    public static UIPersistence Instance;

    public DestinationChoice destinationChoice;
    public GameObject dialoguePanel;
    public GameObject houseAddressPopup; // ADD THIS LINE

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}