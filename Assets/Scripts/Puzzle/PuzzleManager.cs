using UnityEngine;

public class PuzzleManager : MonoBehaviour
{
    [Header("References")]
    public PuzzlePlayer player;
    public PuzzlePlayerMovement playerMovement;
    public GridManager gridManager;
    public PuzzleSpawnPoint spawnPoint;

    [Header("Move Tracking")]
    public int moveCount;

    private void Start()
    {
        SetPlayerSpawn();
        PlayerMoved();
    }

    private void SetPlayerSpawn()
    {
        Vector2Int spawnPosition = spawnPoint.GetPosition();

        player.playerPosition = spawnPosition;

        player.transform.position = new Vector3(
            spawnPosition.x,
            spawnPosition.y,
            player.transform.position.z
        );
    }

    public void TryMovePlayer(Vector2Int targetPosition)
    {
        GridData currentGrid =
            gridManager.GetGridData(player.playerPosition);

        GridData targetGrid =
            gridManager.GetGridData(targetPosition);

        if (currentGrid != null)
        {
            if (targetPosition == player.playerPosition + Vector2Int.up &&
                currentGrid.ladder_Up != null)
            {
                MovePlayer(currentGrid.ladder_Up.GetEndPosition());
                return;
            }

            if (targetPosition == player.playerPosition + Vector2Int.down &&
                currentGrid.ladder_Down != null)
            {
                MovePlayer(currentGrid.ladder_Down.GetStartPosition());
                return;
            }
        }

        if (targetGrid == null)
        {
            StartFall();
            return;
        }

        if (targetGrid.isBlocked)
        {
            return;
        }

        if (targetGrid.hasPlatform)
        {
            MovePlayer(targetPosition);
            return;
        }

        Vector2Int landingPosition = FindPlatformBelow(targetPosition);

        if (landingPosition != targetPosition)
        {
            GridData landingGrid = gridManager.GetGridData(landingPosition);

            if (!landingGrid.isBlocked)
            {
                MovePlayer(landingPosition);
            }
        }
        else
        {
            StartFall();
        }
    }

    private void MovePlayer(Vector2Int targetPosition)
    {
        moveCount++;

        playerMovement.MoveTo(targetPosition);

        PlayerMoved();
    }

    private Vector2Int FindPlatformBelow(Vector2Int startPosition)
    {
        Vector2Int checkPosition =
            startPosition + Vector2Int.down;

        while (checkPosition.y >= -4)
        {
            GridData grid =
                gridManager.GetGridData(checkPosition);

            if (grid != null && grid.hasPlatform)
            {
                return checkPosition;
            }

            checkPosition += Vector2Int.down;
        }

        return startPosition;
    }

    private void StartFall()
    {
        player.isFalling = true;

        Debug.Log("Game Over");
    }

    public void PlayerMoved()
    {
        CheckButtons();
        CheckLadders();
        CheckLevers();
        CheckGoals();
    }

    private void CheckButtons()
    {
        foreach (GridData grid in gridManager.GetAllGridData())
        {
            if (grid.button == null)
                continue;

            if (player.playerPosition == grid.button.GetPosition())
            {
                grid.button.PressButton();
            }
            else
            {
                grid.button.ReleaseButton();
            }
        }
    }

    private void CheckLadders()
    {
        GridData currentGrid =
            gridManager.GetGridData(player.playerPosition);

        if (currentGrid == null)
            return;

        if (currentGrid.ladder_Up != null)
        {
            Debug.Log("Ladder Up");
        }

        if (currentGrid.ladder_Down != null)
        {
            Debug.Log("Ladder Down");
        }
    }

    private void CheckLevers()
    {
    }

    private void CheckGoals()
    {
        GridData currentGrid =
            gridManager.GetGridData(player.playerPosition);

        if (currentGrid != null &&
            currentGrid.goal != null)
        {
            Debug.Log("Reached Goal");
        }
    }

    public void TryMoveDown()
    {
        GridData currentGrid = gridManager.GetGridData(player.playerPosition); 

        if(currentGrid != null && currentGrid.ladder_Down != null)
        {
            MovePlayer(currentGrid.ladder_Down.GetStartPosition());
        }
        return;
    }
}