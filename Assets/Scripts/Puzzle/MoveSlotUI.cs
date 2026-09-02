using UnityEngine;
using UnityEngine.UI;

public class MoveSlotUI : MonoBehaviour
{
    public Image icon_Image;

    public Sprite icon_Default;
    public Sprite icon_MoveLeft;
    public Sprite icon_MoveRight;
    public Sprite icon_MoveUp;
    public Sprite icon_MoveDown;

    private void Start()
    {
        icon_Image = GetComponent<Image>();
        icon_Image.sprite = icon_Default;
    }

    public void SetMove(PuzzlePlayerMoveType moveType)
    {
        GetMoveImage(moveType);
    }

    public void ClearMove()
    {
        icon_Image.sprite = icon_Default;
    }

    public void GetMoveImage(PuzzlePlayerMoveType moveType)
    {
        switch(moveType)
        {
            case PuzzlePlayerMoveType.Up:
                icon_Image.sprite = icon_MoveUp;
                break;
            case PuzzlePlayerMoveType.Down:
                icon_Image.sprite = icon_MoveDown;
                break;
            case PuzzlePlayerMoveType.Left:
                icon_Image.sprite = icon_MoveLeft;
                break;
            case PuzzlePlayerMoveType.Right:
                icon_Image.sprite = icon_MoveRight;
                break;
        }
    }
}
