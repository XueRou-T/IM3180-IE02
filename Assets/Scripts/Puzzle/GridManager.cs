using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridManager : MonoBehaviour
{
    [Header("Platform")]
    public Tilemap floorTileMap;
    public Tilemap wallTileMap;

    private Dictionary<Vector2Int, GridData> gridData = new Dictionary<Vector2Int, GridData>();

    private void Start()
    {
        BuildGridData();
        RegisterPuzzleObjects();
    }

    private void BuildGridData()
    {
        BoundsInt bounds = floorTileMap.cellBounds;

        foreach (Vector3Int position in bounds.allPositionsWithin)
        {
            Vector2Int gridPosition =  new Vector2Int(position.x, position.y);

            GridData data = new GridData();

            data.hasPlatform = floorTileMap.HasTile(position);
            data.isBlocked = wallTileMap.HasTile(position);

            gridData[gridPosition] = data;
        }
    }

    public GridData GetGridData(Vector2Int position)
    {
        if (gridData.TryGetValue(position, out GridData data))
        {
            return data;
        }

        return null;
    }

    public bool HasPlatform(Vector2Int position)
    {
        GridData grid = GetGridData(position);

        if (grid == null)
            return false;

        return grid.hasPlatform;
    }

    public IEnumerable<GridData> GetAllGridData()
    {
        return gridData.Values;
    }

    private void RegisterPuzzleObjects()
    {
        RegisterButtons();
        RegisterDoors();
        RegisterLadders();
        RegisterGoals();
    }

    private void RegisterButtons()
    {
        PuzzleButton[] buttons =
            FindObjectsByType<PuzzleButton>(
                FindObjectsSortMode.None
            );

        foreach (PuzzleButton button in buttons)
        {
            Vector2Int position = button.GetPosition();

            GridData grid = GetGridData(position);

            if (grid == null)
                continue;

            grid.button = button;
        }
        Debug.Log("Registered Buttons");
    }

    private void RegisterDoors()
    {
        PuzzleDoor[] doors =
            FindObjectsByType<PuzzleDoor>(
                FindObjectsSortMode.None
            );

        foreach (PuzzleDoor door in doors)
        {
            Vector2Int position = door.GetPosition();

            GridData grid = GetGridData(position);

            if (grid == null)
                continue;

            grid.door = door;
            
            if(door.doorType == DoorType.Door)
                grid.isBlocked = !door.isOpen;
            else
                grid.hasPlatform = !door.isOpen;
        }
        Debug.Log("Registered Doors");
    }

    private void RegisterLadders()
    {
        PuzzleLadder[] ladders =
            FindObjectsByType<PuzzleLadder>(
                FindObjectsSortMode.None
            );

        foreach (PuzzleLadder ladder in ladders)
        {
            Vector2Int startPosition =
                ladder.GetStartPosition();

            Vector2Int endPosition =
                ladder.GetEndPosition();

            GridData startGrid =
                GetGridData(startPosition);

            GridData endGrid =
                GetGridData(endPosition);

            if (startGrid == null || endGrid == null)
                continue;

            startGrid.ladder_Up = ladder;
            endGrid.ladder_Down = ladder;
        }
        Debug.Log("Registered Ladder");
    }

    private void RegisterGoals()
    {
        PuzzleGoal[] goals =
            FindObjectsByType<PuzzleGoal>(
                FindObjectsSortMode.None
            );

        foreach (PuzzleGoal goal in goals)
        {
            Vector2Int position = goal.GetPosition();

            GridData grid = GetGridData(position);

            if (grid == null)
                continue;

            grid.goal = goal;
        }
        Debug.Log("Registered Goals");
    }

    public Vector2Int GetGridPosition(Vector3 position)
    {
        return new Vector2Int(
            Mathf.RoundToInt(position.x),
            Mathf.RoundToInt(position.y)
        );
    }

    public void SetBlocked(Vector2Int position, bool blocked)
    {
        GridData grid = GetGridData(position);

        if (grid != null)
        {
            grid.isBlocked = blocked;
        }
    }

    public void SetPlatform(Vector2Int position, bool hasPlatform)
    {
        GridData grid = GetGridData(position);

        if (grid != null)
        {
            grid.hasPlatform = hasPlatform;
        }
    }
}