using UnityEngine;

public class PuzzlePlayer : MonoBehaviour
{
    [Header("Player Position")]
    public Vector2Int gridPosition;

    [Header("Player State")]
    public bool isFalling;
    public bool isOnLadder;
}
