using UnityEngine;

public class PuzzleDoor : MonoBehaviour
{
    public bool isOpen;

    public void OpenDoor()
    {
        if (isOpen)
            return;

        isOpen = true;

        gameObject.SetActive(false);
    }

    public void CloseDoor()
    {
        if(!isOpen) 
            return;

        isOpen = false;

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
