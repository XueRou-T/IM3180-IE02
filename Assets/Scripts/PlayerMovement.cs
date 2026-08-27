using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public GridManager gridManager;

    [Header("Grid Position")]
    public Vector2Int gridPosition;
    public Vector2Int targetPosition;

    [Header("Fall Settings")]
    public int minimumY = -5;
    public float fallSpeed = 5f;
    public bool isFalling = false;

    private void Start()
    {
        gridPosition = new Vector2Int(
            Mathf.RoundToInt(transform.position.x),
            Mathf.RoundToInt(transform.position.y)
        );
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            GoLeft();
            Debug.Log("Player went left");
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            GoRight();
            Debug.Log("Player went right");
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            GoUp();
            Debug.Log("Player went up");
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            GoDown();
            Debug.Log("Player went down");
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            JumpLeft();
            Debug.Log("Player jump left");
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            JumpRight();
            Debug.Log("Player jump right");
        }
    }

    void GoLeft()
    {
        targetPosition = gridPosition + Vector2Int.left;
        CheckAndMove(targetPosition);
    }

    void GoRight()
    {
        targetPosition = gridPosition + Vector2Int.right;
        CheckAndMove(targetPosition);
    }

    void GoUp()
    {
        targetPosition = gridPosition + Vector2Int.up;
        CheckAndMove(targetPosition);
    }

    void GoDown()
    {
        targetPosition = gridPosition + Vector2Int.down;
        CheckAndMove(targetPosition);
    }

    void JumpLeft()
    {
        targetPosition = gridPosition + new Vector2Int(-1,1);
        CheckAndMove(targetPosition);
    }

    void JumpRight()
    {
        targetPosition = gridPosition + new Vector2Int(1, 1);
        CheckAndMove(targetPosition);
    }

    public void CheckAndMove(Vector2Int targetPosition)
    {
        if(gridManager.HasPlatform(targetPosition))
        {
            MoveTo(targetPosition);
            return;
        }

        Vector2Int landingPosition = FindPlatformBelow(targetPosition);
        if (landingPosition != targetPosition)
        {
            MoveTo(landingPosition);            
        }
        else
        {
            Debug.Log("Game Over");
        }
    }

    public Vector2Int FindPlatformBelow(Vector2Int startPosition)
    {
        Vector2Int checkPosition = startPosition + Vector2Int.down;

        while(checkPosition.y > minimumY)
        {
            if(gridManager.HasPlatform(checkPosition))
                return checkPosition;

            checkPosition += Vector2Int.down;
        }

        return startPosition;
    }
    public void MoveTo(Vector2Int targetPosition)
    {
        gridPosition = targetPosition;

        transform.position = new Vector3(
                gridPosition.x,
                gridPosition.y,
                0
            );
    }
}
