using UnityEngine;

public class PuzzleLadder : MonoBehaviour
{
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

    public Vector2Int GetStartPosition()
    {
        return ladderPosition_Start;
    }

    public Vector2Int GetEndPosition()
    {
        return ladderPosition_End;
    }
}
