using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    public PuzzlePlayerAnimator playerAnimator;
    public PuzzleShadowAnimator shadowAnimator;
    

    [Header("Move Tracking")]
    public int moveCount;
    public int shadowMoveCount = 5;
    public List<PuzzlePlayerMoveType> moveHistory = new List<PuzzlePlayerMoveType>();
    public MoveTrackerUI moveTrackerUI;
    private bool shadowStarted;
    private int shadowMoveIndex;
    private bool gameOver = false;

    void Start()
    {
        SetPlayerSpawn();
        Invoke(nameof(PlayerMoved), 0.1f);
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

        if (HasPlatformAfterMove(targetPosition, targetPosition, shadow.shadowPosition))
        {
            MovePlayer(targetPosition, moveType);
            return;
        }

        Vector2Int landingPosition = FindPlatformBelow(targetPosition, targetPosition, shadow.shadowPosition);

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

        if (moveType == PuzzlePlayerMoveType.Left)
        {
            playerAnimator.MoveLeft();
        }
        else if (moveType == PuzzlePlayerMoveType.Right)
        {
            playerAnimator.MoveRight();
        }
        if (moveType == PuzzlePlayerMoveType.Down)
        {
            player.isFalling = false;
            player.isClimbing = true;
            playerAnimator.MoveDown();

        }
        else if (moveType == PuzzlePlayerMoveType.Up)
        {
            player.isFalling = false;
            player.isClimbing = true;
            playerAnimator.MoveUp();
        }
        else if (targetPosition.y < player.playerPosition.y)
        {
            player.isFalling = true;
            player.isClimbing = false;
        }

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

        PuzzlePlayerMoveType move = moveHistory[shadowMoveIndex];

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
                shadowAnimator.MoveLeft();
                break;
            case PuzzlePlayerMoveType.Right:
                shadowTargetPosition += Vector2Int.right;
                shadowAnimator.MoveRight();
                break;
            case PuzzlePlayerMoveType.Up:
                if (shadowCurrentGrid != null && shadowCurrentGrid.ladder_Up != null)
                {
                    if (!IsDoorBlockedAfterMove(shadowCurrentGrid.ladder_Up.GetEndPosition(), player.playerPosition, shadowCurrentGrid.ladder_Up.GetEndPosition()))
                    {
                        shadowMovement.MoveTo(shadowCurrentGrid.ladder_Up.GetEndPosition());
                        shadowAnimator.MoveUp();
                    }

                    return;
                }
                
                shadowTargetPosition += Vector2Int.up;
               

                break;

            case PuzzlePlayerMoveType.Down:
                if(shadowCurrentGrid != null && shadowCurrentGrid.ladder_Down != null)
                {
                    if (!IsDoorBlockedAfterMove(shadowCurrentGrid.ladder_Down.GetStartPosition(), player.playerPosition, shadowCurrentGrid.ladder_Down.GetStartPosition()))
                    {
                        shadowMovement.MoveTo(shadowCurrentGrid.ladder_Down.GetStartPosition());
                        shadowAnimator.MoveDown();
                    }
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

        Vector2Int landingPosition = FindPlatformBelow(shadowTargetPosition, player.playerPosition, shadowTargetPosition);

        if (landingPosition == shadowTargetPosition)
            return;

        GridData landingGrid = gridManager.GetGridData(landingPosition);

        if (landingGrid == null)
            return;

        if (IsDoorBlockedAfterMove(landingPosition, player.playerPosition, landingPosition))
            return;

        shadowMovement.MoveTo(landingPosition);
    }

    Vector2Int FindPlatformBelow(Vector2Int startPosition, Vector2Int nextPlayerPosition, Vector2Int nextShadowPosition)
    {
        Vector2Int checkPosition = startPosition + Vector2Int.down;

        while (checkPosition.y >= -4)
        {
            GridData grid = gridManager.GetGridData(checkPosition);

            if (HasPlatformAfterMove(checkPosition, nextPlayerPosition, nextShadowPosition))
            {
                return checkPosition;
            }

            checkPosition += Vector2Int.down;
        }

        return startPosition;
    }

    void StartFall()
    {
        if (player.isFalling)
            return;

        player.isFalling = true;

        Debug.Log("Game Over");
    }

    public void PlayerMoved()
    {
        CheckButtons();
        CheckLadders();
        CheckGoals();
        CheckPlayerStandingOnPlatform();
        CheckPlayerShadowCollide();
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

        if ((targetGrid.door.doorType == DoorType.Trapdoor))
            return false;

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

    bool HasPlatformAfterMove(Vector2Int targetPosition, Vector2Int nextPlayerPosition, Vector2Int nextShadowPosition)
    {
        GridData targetGrid = gridManager.GetGridData(targetPosition);

        if (targetGrid == null)
            return false;

        if (targetGrid.door != null && targetGrid.door.doorType == DoorType.Trapdoor)
        {
            return targetGrid.hasPlatform;
        }

        if (targetGrid.door == null)
            return targetGrid.hasPlatform;

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

        return targetGrid.hasPlatform;
    }

    void CheckPlayerStandingOnPlatform()
    {
        GridData currentGrid = gridManager.GetGridData(player.playerPosition);

        if (currentGrid == null)
            return;

        if (currentGrid.hasPlatform)
            return;

        Vector2Int landingPosition = FindPlatformBelow(player.playerPosition, player.playerPosition, shadow.shadowPosition
        );

        if (landingPosition == player.playerPosition)
        {
            StartFall();
            return;
        }

        if (IsDoorBlockedAfterMove(landingPosition, landingPosition, shadow.shadowPosition))
        {
            StartFall();
            return;
        }

        playerMovement.MoveTo(landingPosition);
    }

    void CheckPlayerShadowCollide()
    {
        if (gameOver)
            return;

        if(player.playerPosition == shadow.shadowPosition && !gameOver)
        {
            Debug.Log("Game Over: Collided");
        }
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}