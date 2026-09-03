using UnityEngine;
using System.Collections.Generic;

public class PuzzleManager : MonoBehaviour
{
    [Header("References")]
    public PuzzlePlayer player;
    public PuzzlePlayerMovement playerMovement;
    public PuzzleShadow shadow;
    public PuzzleShadowMovement shadowMovement;
    public GridManager gridManager;
    public PuzzleSpawnPoint spawnPoint;
    public PuzzleSpawnPoint shadowSpawnPoint;
    

    [Header("Move Tracking")]
    public int moveCount;
    public int shadowMoveCount = 5;
    public List<PuzzlePlayerMoveType> moveHistory = new List<PuzzlePlayerMoveType>();
    public MoveTrackerUI moveTrackerUI;
    private bool shadowStarted;
    private int shadowMoveIndex;

    void Start()
    {
        SetPlayerSpawn();
        PlayerMoved();
    }

    void SetPlayerSpawn()
    {
        Vector2Int spawnPosition = spawnPoint.GetPosition();

        player.playerPosition = spawnPosition;

        player.transform.position = new Vector3(
            spawnPosition.x,
            spawnPosition.y,
            player.transform.position.z
        );
    }

    void SpawnShadow()
    {
        shadow.gameObject.SetActive(true);

        Vector2Int spawnPosition = shadowSpawnPoint.GetPosition();

        shadow.shadowPosition = spawnPosition;

        shadow.transform.position = new Vector3(
            spawnPosition.x,
            spawnPosition.y,
            shadow.transform.position.z
        );
    }


    public void TryMovePlayer(Vector2Int targetPosition, PuzzlePlayerMoveType moveType)
    {
        GridData currentGrid = gridManager.GetGridData(player.playerPosition);

        GridData targetGrid = gridManager.GetGridData(targetPosition);

        if (currentGrid != null)
        {
            if (targetPosition == player.playerPosition + Vector2Int.up && currentGrid.ladder_Up != null)
            {
                targetGrid = gridManager.GetGridData(currentGrid.ladder_Up.GetEndPosition());
                if (!IsDoorBlockedAfterMove(currentGrid.ladder_Up.GetEndPosition(), currentGrid.ladder_Up.GetEndPosition(), shadow.shadowPosition))
                {
                    MovePlayer(currentGrid.ladder_Up.GetEndPosition(), PuzzlePlayerMoveType.Up);
                    return;
                }
            }

            if (targetPosition == player.playerPosition + Vector2Int.down && currentGrid.ladder_Down != null)
            {
                targetGrid = gridManager.GetGridData(currentGrid.ladder_Down.GetStartPosition());
                if (!IsDoorBlockedAfterMove(currentGrid.ladder_Down.GetStartPosition(), currentGrid.ladder_Down.GetStartPosition(), shadow.shadowPosition))
                {
                    MovePlayer(currentGrid.ladder_Down.GetStartPosition(), PuzzlePlayerMoveType.Down);
                    return;
                }
            }
        }

        if (targetGrid == null)
        {
            StartFall();
            return;
        }

        if(moveType == PuzzlePlayerMoveType.Up)
        {
            return;
        }

        if (IsDoorBlockedAfterMove(targetPosition, targetPosition, shadow.shadowPosition))
        {
            return;
        }

        if (targetGrid.hasPlatform)
        {
            MovePlayer(targetPosition, moveType);
            return;
        }

        Vector2Int landingPosition = FindPlatformBelow(targetPosition);

        if (landingPosition != targetPosition)
        {
            GridData landingGrid = gridManager.GetGridData(landingPosition);

            if (!IsDoorBlockedAfterMove(landingPosition, landingPosition, shadow.shadowPosition))
            {
                MovePlayer(landingPosition, moveType);
            }
        }
        else
        {
            StartFall();
        }
    }

    void MovePlayer(Vector2Int targetPosition, PuzzlePlayerMoveType moveType)
    {
        moveCount++;

        playerMovement.MoveTo(targetPosition);

        if (moveHistory.Count < shadowMoveCount)
        {
            moveHistory.Add(moveType);
            moveTrackerUI.UpdateMoveSlots();

            if (moveHistory.Count == shadowMoveCount)
            {
                SpawnShadow();
                shadowStarted = true;
            }
        }

        PlayerMoved();

        if (moveCount > shadowMoveCount && shadowStarted)
        {
            ExecuteNextShadowMove();

            PlayerMoved();
        }
    }

    void ExecuteNextShadowMove()
    {
        if (shadowMoveIndex >= moveHistory.Count)
            return;

        PuzzlePlayerMoveType move =
            moveHistory[shadowMoveIndex];

        shadowMoveIndex++;

        TryMoveShadow(move);

    }

    public void TryMoveShadow(PuzzlePlayerMoveType moveType)
    {
        Vector2Int shadowCurrentPosition = shadow.shadowPosition;

        GridData shadowCurrentGrid = gridManager.GetGridData(shadowCurrentPosition);

        Vector2Int shadowTargetPosition = shadowCurrentPosition;

        switch(moveType)
        {
            case PuzzlePlayerMoveType.Left:
                shadowTargetPosition += Vector2Int.left;
                break;
            case PuzzlePlayerMoveType.Right:
                shadowTargetPosition += Vector2Int.right;
                break;
            case PuzzlePlayerMoveType.Up:
                if (shadowCurrentGrid != null && shadowCurrentGrid.ladder_Up != null)
                {
                    if(!IsDoorBlockedAfterMove(shadowCurrentGrid.ladder_Up.GetEndPosition(), player.playerPosition, shadowCurrentGrid.ladder_Up.GetEndPosition()))
                        shadowMovement.MoveTo(shadowCurrentGrid.ladder_Up.GetEndPosition());

                    return;
                }
                
                shadowTargetPosition += Vector2Int.up;
                
                break;

            case PuzzlePlayerMoveType.Down:
                if(shadowCurrentGrid != null && shadowCurrentGrid.ladder_Down != null)
                {
                    if(!IsDoorBlockedAfterMove(shadowCurrentGrid.ladder_Down.GetStartPosition(), player.playerPosition, shadowCurrentGrid.ladder_Down.GetStartPosition()))
                        shadowMovement.MoveTo(shadowCurrentGrid.ladder_Down.GetStartPosition());
                }
                return;
        }

        GridData targetGrid = gridManager.GetGridData(shadowTargetPosition);

        if (targetGrid == null)
            return;

        if (IsDoorBlockedAfterMove(shadowTargetPosition, player.playerPosition, shadowTargetPosition))
            return;

        if(targetGrid.hasPlatform)
        {
            shadowMovement.MoveTo(shadowTargetPosition);
            return;
        }

        Vector2Int landingPosition = FindPlatformBelow(shadowTargetPosition);

        if (landingPosition == shadowTargetPosition)
            return;

        GridData landingGrid = gridManager.GetGridData(landingPosition);

        if (landingGrid == null)
            return;

        if (IsDoorBlockedAfterMove(landingPosition, player.playerPosition, landingPosition))
            return;

        shadowMovement.MoveTo(landingPosition);
    }

    Vector2Int FindPlatformBelow(Vector2Int startPosition)
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

    void StartFall()
    {
        player.isFalling = true;

        Debug.Log("Game Over");
    }

    public void PlayerMoved()
    {
        CheckButtons();
        CheckLadders();
        CheckGoals();
    }

    void CheckButtons()
    {
        foreach (GridData grid in gridManager.GetAllGridData())
        {
            if (grid.button == null)
                continue;

            if (player.playerPosition == grid.button.GetPosition() || shadow.shadowPosition == grid.button.GetPosition())
            {
                grid.button.PressButton();
            }
            else
            {
                grid.button.ReleaseButton();
            }
        }
    }

    void CheckLadders()
    {
        GridData currentGrid = gridManager.GetGridData(player.playerPosition);

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

    void CheckGoals()
    {
        GridData currentGrid = gridManager.GetGridData(player.playerPosition);

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
            MovePlayer(currentGrid.ladder_Down.GetStartPosition(), PuzzlePlayerMoveType.Down);
        }
        return;
    }

    bool IsDoorBlockedAfterMove(Vector2Int targetPosition, Vector2Int nextPlayerPosition, Vector2Int nextShadowPosition)
    {
        GridData targetGrid = gridManager.GetGridData(targetPosition);

        if (targetGrid == null)
            return true;

        if (targetGrid.door == null)
            return targetGrid.isBlocked;

        foreach (GridData grid in gridManager.GetAllGridData())
        {
            if (grid.button == null)
                continue;

            if (grid.button.connectedDoor != targetGrid.door)
                continue;

            Vector2Int buttonPosition = grid.button.GetPosition();

            bool playerOnButton = nextPlayerPosition == buttonPosition;

            bool shadowOnButton = nextShadowPosition == buttonPosition;

            if (playerOnButton || shadowOnButton)
                return false;
        }

        return !targetGrid.door.isOpen;
    }
}