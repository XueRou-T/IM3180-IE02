using UnityEngine;
using UnityEngine.InputSystem;

public class PuzzlePlayerMovement : MonoBehaviour
{
    public PuzzlePlayer player;
    public PuzzleManager puzzleManager;

    private Vector2Int targetPosition;

    [Header("Fall Settings")]
    public int minimumY = -5;
    public float fallSpeed = 5f;

    void Update()
    {
        if (player.isFalling || player.isClimbing)
            return;

        if (Input.GetKeyDown(KeyCode.A))
        {
            GoLeft();
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            GoRight();
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            GoUp();
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            GoDown();
        }
    }

    void GoLeft()
    {
        targetPosition = player.playerPosition + Vector2Int.left;
        TryMove(targetPosition, PuzzlePlayerMoveType.Left);
    }

    void GoRight()
    {
        targetPosition = player.playerPosition + Vector2Int.right;
        TryMove(targetPosition, PuzzlePlayerMoveType.Right);
    }

    void GoUp()
    {
        targetPosition = player.playerPosition + Vector2Int.up;
        TryMove(targetPosition, PuzzlePlayerMoveType.Up);   
    }

    void GoDown()
    {
        puzzleManager.TryMoveDown();
    }

    public void TryMove(Vector2Int targetPosition, PuzzlePlayerMoveType moveType)
    {
        puzzleManager.TryMovePlayer(targetPosition, moveType);
    }

    public void MoveTo(Vector2Int targetPosition)
    {
        player.playerPosition = targetPosition;

        transform.position = new Vector3(
            targetPosition.x,
            targetPosition.y,
            0
        );
    }
}
