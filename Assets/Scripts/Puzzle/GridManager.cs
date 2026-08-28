using UnityEngine;
using UnityEngine.Tilemaps;

public class GridManager : MonoBehaviour
{
    public Tilemap tilemap;

    public bool HasPlatform(Vector2Int gridPosition)
    {
        Vector3Int cellPosition = new Vector3Int(
            gridPosition.x,
            gridPosition.y,
            0
        );

        return tilemap.HasTile(cellPosition);
    }
}
