using UnityEngine;

public class PuzzlePlayer : MonoBehaviour
{
    [Header("Player Position")]
    public Vector2Int playerPosition;

    [Header("Player State")]
    public bool isFalling;
    public bool isClimbing;
    public bool isUpLadder;
    public bool isDownLadder;

    public Vector2Int GetPosition()
    {
        return new Vector2Int(
            Mathf.RoundToInt(transform.position.x),
            Mathf.RoundToInt(transform.position.y)
        );
    }
}
