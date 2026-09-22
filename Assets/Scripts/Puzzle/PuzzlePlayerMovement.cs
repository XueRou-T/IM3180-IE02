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

    [Header("Move Settings")]
    public float moveSpeed = 8.0f;
    private bool isMoving;

    void Update()
    {
        if (isMoving)
        {
            if (transform.position.x != player.playerPosition.x)
            {
                transform.position = Vector3.MoveTowards(
                    transform.position,
                    new Vector3(
                        player.playerPosition.x,
                        transform.position.y,
                        transform.position.z
                    ),
                    moveSpeed * Time.deltaTime
                );
            }
            else
            {
                transform.position = Vector3.MoveTowards(
                    transform.position,
                    new Vector3(
                        transform.position.x,
                        player.playerPosition.y,
                        transform.position.z
                    ),
                    moveSpeed * Time.deltaTime
                );
            }

            if (transform.position == new Vector3(player.playerPosition.x, player.playerPosition.y, transform.position.z))
            {
                isMoving = false;
                player.isFalling = false;
                player.isClimbing = false;

                puzzleManager.playerAnimator.StopAllAnimation();
            }

            return;
        }

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

        isMoving = true;
        /*transform.position = new Vector3(
            targetPosition.x,
            targetPosition.y,
            0
        );*/
    }
}
