using UnityEngine;

public class PuzzleButton : MonoBehaviour
{
    public PuzzlePlayer player;
    public PuzzleDoor connectedDoor;

    public bool isPressed;

    public Vector2Int buttonPosition;

    public Sprite buttonPressedSprite;
    public Sprite buttonReleasedSprite;

    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        buttonPosition = new Vector2Int(
            Mathf.RoundToInt(transform.position.x),
            Mathf.RoundToInt(transform.position.y) 
        );

        spriteRenderer.sprite = buttonReleasedSprite;
    }

    public void PressButton()
    {
        if (isPressed)
            return;

        isPressed = true;
        spriteRenderer.sprite = buttonPressedSprite;

        if (connectedDoor != null)
        {
            connectedDoor.ToggleDoor();
        }
    }

    public void ReleaseButton()
    {
        if (!isPressed)
            return;

        isPressed = false;
        spriteRenderer.sprite = buttonReleasedSprite;

        if(connectedDoor != null)
        {
            connectedDoor.ToggleDoor();
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
