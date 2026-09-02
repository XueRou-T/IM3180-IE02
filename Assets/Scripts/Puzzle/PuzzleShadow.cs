using UnityEngine;

public class PuzzleShadow : MonoBehaviour
{
    [Header("Shadow Position")]
    public Vector2Int shadowPosition;

    [Header("Shadow State")]
    public bool isFalling;
    public bool isClimbing;
    public bool isUpLadder;
    public bool isDownLadder;

    void Start()
    {
        gameObject.SetActive(false); 
    }

    public Vector2Int GetPosition()
    {
        return new Vector2Int(
            Mathf.RoundToInt(transform.position.x),
            Mathf.RoundToInt(transform.position.y)
        );
    }
}
