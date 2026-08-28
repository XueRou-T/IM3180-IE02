using UnityEngine;

public class PuzzleGoal : MonoBehaviour
{
    public PuzzlePlayer player;

    private Vector2Int goalPosition;

    void Start()
    {
        goalPosition = new Vector2Int(
            Mathf.RoundToInt(transform.position.x),
            Mathf.RoundToInt(transform.position.y)
        );
    }

    void Update()
    {
        if (player.gridPosition == goalPosition)
        {
            Debug.Log("Player completed level");
        }
    }
}
