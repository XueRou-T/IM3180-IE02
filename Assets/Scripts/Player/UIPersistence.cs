using UnityEngine;

public class UIPersistence : MonoBehaviour
{
    public static UIPersistence Instance;

    [Header("Drag the child panels here")]
    public DestinationChoice destinationChoice;
    public DialogueSystem dialogueSystem;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Safe because this object is already at the root level!
        }
        else
        {
            Destroy(gameObject);
        }
    }
}