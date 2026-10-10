using UnityEngine;

public class PuzzleDoor : MonoBehaviour
{
    public bool initiallyOpen;
    public bool isOpen;
    public GridManager gridManager;

    public DoorType doorType;

    public Sprite trapdoorOpened;
    public Sprite trapdoorClosed;
    public Sprite doorOpened;
    public Sprite doorClosed;

    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        isOpen = initiallyOpen;
        
        if(doorType == DoorType.Door)
        {
            gridManager.SetBlocked(GetPosition(), !isOpen);
        }
        else if(doorType == DoorType.Trapdoor)
        {
            gridManager.SetPlatform(GetPosition(), !isOpen);
        }

        if (isOpen) gameObject.SetActive(false);
        else gameObject.SetActive(true);
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
            spriteRenderer.sprite = doorOpened;
        }
        else if (doorType == DoorType.Trapdoor)
        {
            gridManager.SetPlatform(GetPosition(), false);
            spriteRenderer.sprite = trapdoorOpened;
        }
    }

    public void CloseDoor()
    {
        if(!isOpen) 
            return;

        isOpen = false;

        if (doorType == DoorType.Door)
        {
            gridManager.SetBlocked(GetPosition(), true);
            spriteRenderer.sprite = doorClosed;
        }
        else if (doorType==DoorType.Trapdoor)
        {
            gridManager.SetPlatform(GetPosition(), true);
            spriteRenderer.sprite = trapdoorClosed;
        }
    }

    public Vector2Int GetPosition()
    {
        return new Vector2Int(
            Mathf.RoundToInt(transform.position.x),
            Mathf.RoundToInt(transform.position.y)
        );
    }
}
