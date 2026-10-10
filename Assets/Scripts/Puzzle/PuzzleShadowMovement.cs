using UnityEngine;

public class PuzzleShadowMovement : MonoBehaviour
{
    public PuzzleShadow shadow;

    private void Start()
    {
        shadow = GetComponent<PuzzleShadow>();
    }
    public void MoveTo(Vector2Int targetPosition)
    {
        shadow.shadowPosition = targetPosition;

        transform.position = new Vector3(
            targetPosition.x,
            targetPosition.y,
            0
        );
    }
}
