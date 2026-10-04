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
    public MinigameSFXManager minigameSFXManager;

    [Header("Move Tracking")]
    public int moveCount;
    public int shadowMoveCount = 5;
    public List<PuzzlePlayerMoveType> moveHistory = new List<PuzzlePlayerMoveType>();
    public MoveTrackerUI moveTrackerUI;

    private bool shadowStarted;
    private int shadowMoveIndex;
    private bool gameOver = false;
    private PuzzlePlayerMoveType lastMoveType;
    private Vector2Int lastPlayerPos;
    private Vector2Int lastShadowPos;

    [Header("Next Scene")]
    public string nextSceneName;
    void Start()
    {
        SetPlayerSpawn();
        Invoke(nameof(ProcessTurnLoop), 0.1f);
    }

    void SetPlayerSpawn()
    {
        Vector2Int spawnPosition = spawnPoint.GetPosition();
        player.playerPosition = spawnPosition;
        player.transform.position = new Vector3(spawnPosition.x, spawnPosition.y, player.transform.position.z);
    }

    void SpawnShadow()
    {
        shadow.gameObject.SetActive(true);
        Vector2Int spawnPosition = shadowSpawnPoint.GetPosition();
        shadow.shadowPosition = spawnPosition;
        shadow.transform.position = new Vector3(spawnPosition.x, spawnPosition.y, shadow.transform.position.z);
    }

    public void TryMovePlayer(Vector2Int targetPosition, PuzzlePlayerMoveType moveType)
    {
        if (gameOver) return;

        GridData currentGrid = gridManager.GetGridData(player.playerPosition);
        lastPlayerPos = player.playerPosition;

        if (currentGrid != null)
        {
            if (targetPosition == player.playerPosition + Vector2Int.up && currentGrid.ladder_Up != null)
            {
                ExecutePlayerMove(currentGrid.ladder_Up.GetEndPosition(), moveType);
                return;
            }

            if (targetPosition == player.playerPosition + Vector2Int.down && currentGrid.ladder_Down != null)
            {
                ExecutePlayerMove(currentGrid.ladder_Down.GetStartPosition(), moveType);
                return;
            }
        }

        if (moveType == PuzzlePlayerMoveType.Up || moveType == PuzzlePlayerMoveType.Down) return;

        ExecutePlayerMove(targetPosition, moveType);
    }

    void ExecutePlayerMove(Vector2Int targetPosition, PuzzlePlayerMoveType moveType)
    {
        moveCount++;
        lastMoveType = moveType;

        if (moveType == PuzzlePlayerMoveType.Left)
        {
            playerAnimator.MoveLeft();
            minigameSFXManager.PlayWalkingSound();
        }
        else if (moveType == PuzzlePlayerMoveType.Right)
        {
            playerAnimator.MoveRight();
            minigameSFXManager.PlayWalkingSound();
        }

        if (moveType == PuzzlePlayerMoveType.Down)
        {
            player.isFalling = false;
            player.isClimbing = true;
            playerAnimator.MoveDown();
            minigameSFXManager.PlayWalkingSound();
        }
        else if (moveType == PuzzlePlayerMoveType.Up)
        {
            player.isFalling = false;
            player.isClimbing = true;
            playerAnimator.MoveUp();
            minigameSFXManager.PlayWalkingSound();
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

        if (moveCount > shadowMoveCount && shadowStarted)
        {
            ExecuteNextShadowMove();
        }

        ProcessTurnLoop();
    }

    void ExecuteNextShadowMove()
    {
        if (shadowMoveIndex >= moveHistory.Count) return;

        PuzzlePlayerMoveType move = moveHistory[shadowMoveIndex];
        shadowMoveIndex++;

        TryMoveShadow(move);
    }

    public void TryMoveShadow(PuzzlePlayerMoveType moveType)
    {
        Vector2Int shadowCurrentPosition = shadow.shadowPosition;
        GridData shadowCurrentGrid = gridManager.GetGridData(shadowCurrentPosition);
        Vector2Int shadowTargetPosition = shadowCurrentPosition;

        lastShadowPos = shadowCurrentPosition;

        switch (moveType)
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
                    shadowMovement.MoveTo(shadowCurrentGrid.ladder_Up.GetEndPosition());
                    shadowAnimator.MoveUp();
                    return;
                }
                break;

            case PuzzlePlayerMoveType.Down:
                if (shadowCurrentGrid != null && shadowCurrentGrid.ladder_Down != null)
                {
                    shadowMovement.MoveTo(shadowCurrentGrid.ladder_Down.GetStartPosition());
                    shadowAnimator.MoveDown();
                    return;
                }
                break;
        }

        shadowMovement.MoveTo(shadowTargetPosition);
    }

    public void ProcessTurnLoop()
    {
        if (gameOver) return;

        bool loop;
        int maxIterations = 20;
        int iteration = 0;

        do
        {
            loop = false;
            iteration++;

            CheckButtons();

            GridData playerGrid = gridManager.GetGridData(player.playerPosition);
            if (playerGrid == null || !IsSolidPlatform(playerGrid))
            {
                Vector2Int nextPlatform = FindPlatformStepped(player.playerPosition);

                if (nextPlatform != player.playerPosition)
                {
                    playerMovement.MoveTo(nextPlatform);
                    GridData landedGrid = gridManager.GetGridData(nextPlatform);

                    if (landedGrid != null && landedGrid.button != null)
                    {
                        loop = true;
                    }
                }
                else
                {
                    StartFall(lastMoveType);
                    return;
                }
            }

            if (shadowStarted)
            {
                GridData shadowGrid = gridManager.GetGridData(shadow.shadowPosition);
                if (shadowGrid == null || !IsSolidPlatform(shadowGrid))
                {
                    Vector2Int nextPlatform = FindPlatformStepped(shadow.shadowPosition);

                    if (nextPlatform != shadow.shadowPosition)
                    {
                        shadowMovement.MoveTo(nextPlatform);
                        GridData landedGrid = gridManager.GetGridData(nextPlatform);

                        if (landedGrid != null && landedGrid.button != null)
                        {
                            loop = true;
                        }
                    }
                }
            }

            if (loop) continue;

        } while (loop && iteration < maxIterations);

        CheckPlayerClosedDoorDeath();
        CheckLadders();
        CheckGoals();
        CheckPlayerShadowCollide();
        CheckShadowlosedDoorDeath();
    }

    void CheckButtons()
    {
        foreach (GridData grid in gridManager.GetAllGridData())
        {
            if (grid.button == null) continue;

            Vector2Int buttonPos = grid.button.GetPosition();
            bool isPlayerOn = player.playerPosition == buttonPos;
            bool isShadowOn = shadowStarted && shadow.shadowPosition == buttonPos;

            if (isPlayerOn || isShadowOn)
            {
                grid.button.PressButton();
            }
            else
            {
                grid.button.ReleaseButton();
            }
        }
    }

    Vector2Int FindPlatformStepped(Vector2Int startPosition)
    {
        Vector2Int checkPosition = startPosition + Vector2Int.down;

        while (checkPosition.y >= -10)
        {
            GridData grid = gridManager.GetGridData(checkPosition);

            if (grid != null)
            {
                if (grid.door != null && grid.door.doorType == DoorType.Trapdoor)
                {
                    if (!grid.door.isOpen)
                    {
                        return checkPosition;
                    }
                }
                else if (grid.hasPlatform)
                {
                    return checkPosition;
                }
            }

            checkPosition += Vector2Int.down;
        }

        return startPosition;
    }

    bool IsSolidPlatform(GridData grid)
    {
        if (grid == null) return false;

        if (grid.door != null && grid.door.doorType == DoorType.Trapdoor)
        {
            return !grid.door.isOpen;
        }

        return grid.hasPlatform;
    }

    void CheckPlayerClosedDoorDeath()
    {
        GridData currentGrid = gridManager.GetGridData(player.playerPosition);
        if (currentGrid == null || currentGrid.door == null) return;

        if (currentGrid.door.doorType == DoorType.Door && !currentGrid.door.isOpen)
        {
            Debug.Log("Game Over: Player trapped inside a closed death door!");
            gameOver = true;
            StartFall(lastMoveType);
        }
    }

    void CheckShadowlosedDoorDeath()
    {
        GridData currentGrid = gridManager.GetGridData(shadow.shadowPosition);
        if (currentGrid == null || currentGrid.door == null) return;

        if (currentGrid.door.doorType == DoorType.Door && !currentGrid.door.isOpen)
        {
            Debug.Log("Shadow gone");
            shadow.gameObject.SetActive(false);
        }
    }

    void CheckPlayerShadowCollide()
    {
        if (gameOver) return;

        if (player.playerPosition == shadow.shadowPosition || (player.playerPosition == lastShadowPos && shadow.shadowPosition == lastPlayerPos) )
        {
            gameOver = true;
            Debug.Log("Game Over: Collided with Shadow");
            StartFall(lastMoveType);
        }
    }

    void CheckLadders()
    {
        GridData currentGrid = gridManager.GetGridData(player.playerPosition);
        if (currentGrid == null) return;

        if (currentGrid.ladder_Up != null) Debug.Log("Ladder Up");
        if (currentGrid.ladder_Down != null) Debug.Log("Ladder Down");
    }

    void CheckGoals()
    {
        GridData currentGrid = gridManager.GetGridData(player.playerPosition);
        if (currentGrid != null && currentGrid.goal != null)
        {
            Debug.Log("Reached Goal");
            GoToNextScene();
        }
    }

    void StartFall(PuzzlePlayerMoveType direction)
    {
        if (player.isFalling) return;

        player.isFalling = true;
        gameOver = true;

        if (direction == PuzzlePlayerMoveType.Left) playerAnimator.MoveLeft();
        if (direction == PuzzlePlayerMoveType.Right) playerAnimator.MoveRight();

        playerMovement.FallTo(direction);
        Debug.Log("Game Over");
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void GoToNextScene()
    {
        SceneManager.LoadScene(nextSceneName);
    }
}