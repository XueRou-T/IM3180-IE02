using UnityEngine;

public class PuzzleSpawnPoint : MonoBehaviour
{
    public PuzzlePlayer player;
    //public GameObject shadow;

    void Start()
    {
        Vector3 spawnPosition = transform.position;

        player.transform.position = spawnPosition;
        //shadow.transform.position = spawnPosition;

        player.gridPosition = new Vector2Int(
            Mathf.RoundToInt(spawnPosition.x),
            Mathf.RoundToInt(spawnPosition.y)
        );
    }
}
