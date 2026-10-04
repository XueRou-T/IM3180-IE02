using UnityEngine;

public class UIPersistence : MonoBehaviour
{
    public static UIPersistence Instance;

    [Header("Drag the child panels here")]
    public LaptopLogin laptopLogin;
    public LaptopPuzzle laptopPuzzle;
    public DestinationChoice destinationChoice;

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