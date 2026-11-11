using UnityEngine;

public class UIPersistence : MonoBehaviour
{
    public static UIPersistence Instance;

    public DestinationChoice destinationChoice;
    public GameObject dialoguePanel;
    public GameObject houseAddressPopup;
    [HideInInspector] public AddressPopupController addressPopupController;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            if (houseAddressPopup == null)
            {
                Transform foundPopup = transform.Find("House Address Popup");
                if (foundPopup != null) houseAddressPopup = foundPopup.gameObject;
            }

            if (houseAddressPopup != null)
            {
                addressPopupController = houseAddressPopup.GetComponent<AddressPopupController>();
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }
}