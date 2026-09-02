using UnityEngine;

public class PuzzleDoor : MonoBehaviour
{
    public bool isOpen;
    public GridManager gridManager;

    public void OpenDoor()
    {
        if (isOpen)
            return;

        isOpen = true;

        gridManager.SetBlocked(GetPosition(), false);

        gameObject.SetActive(false);
    }

    public void CloseDoor()
    {
        if(!isOpen) 
            return;

        isOpen = false;

        gridManager.SetBlocked(GetPosition(), true);

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
