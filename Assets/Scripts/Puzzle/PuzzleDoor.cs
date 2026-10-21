using UnityEngine;

public class PuzzleDoor : MonoBehaviour
{
    public bool initiallyOpen;
    public bool isOpen;
    public GridManager gridManager;

    public DoorType doorType;

    void Start()
    {
        isOpen = initiallyOpen;
        
        if(doorType == DoorType.Door)
        {
            gridManager.SetBlocked(GetPosition(), !isOpen);
        }
        else if(doorType == DoorType.Trapdoor)
        {
            gridManager.SetPlatform(GetPosition(), !isOpen);
        }
    }
    public void ToggleDoor()
    {
        if(isOpen)
            CloseDoor();
        else
            OpenDoor();
    }
    public void OpenDoor()
    {
        if (isOpen)
            return;

        isOpen = true;

        if (doorType == DoorType.Door)
        {
            gridManager.SetBlocked(GetPosition(), false);
        }
        else if (doorType == DoorType.Trapdoor)
        {
            gridManager.SetPlatform(GetPosition(), false);
        }

            gameObject.SetActive(false);
    }

    public void CloseDoor()
    {
        if(!isOpen) 
            return;

        isOpen = false;

        if (doorType == DoorType.Door)
        {
            gridManager.SetBlocked(GetPosition(), true);
        }
        else if (doorType==DoorType.Trapdoor)
        {
            gridManager.SetPlatform(GetPosition(), true);
        }

            gameObject.SetActive(true);
    }

    public Vector2Int GetPosition()
    {
        return new Vector2Int(
            Mathf.RoundToInt(transform.position.x),
            Mathf.RoundToInt(transform.position.y)
        );
    }
}
