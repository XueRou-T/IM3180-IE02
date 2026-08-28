using UnityEngine;

public class PuzzleLadder : MonoBehaviour
{
    public PuzzlePlayer player;

    public Vector2Int ladderPosition_Start;
    public Vector2Int ladderPosition_End;

    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        ladderPosition_Start = new Vector2Int(
            Mathf.RoundToInt(transform.position.x),
            Mathf.RoundToInt(transform.position.y)
        );

        int ladderLength = Mathf.RoundToInt(spriteRenderer.size.y);

        ladderPosition_End = ladderPosition_Start + Vector2Int.up * ladderLength;
    }

    void Update()
    {
        if(player.gridPosition == ladderPosition_Start || player.gridPosition == ladderPosition_End)
        {
            player.isOnLadder = true;
        }
        else
        {
            player.isOnLadder = false;
        }


    }
}
