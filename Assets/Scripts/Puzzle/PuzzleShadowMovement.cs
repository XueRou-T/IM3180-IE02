using UnityEngine;

public class PuzzleShadowMovement : MonoBehaviour
{
    public PuzzleShadow shadow;
    public PuzzleShadowAnimator animator;

    [Header("Move Settings")]
    public float moveSpeed = 8.0f;
    private bool isMoving;

    private void Start()
    {
        shadow = GetComponent<PuzzleShadow>();
    }

    void Update()
    {
        if (isMoving)
        {
            if (transform.position.x != shadow.shadowPosition.x)
            {
                transform.position = Vector3.MoveTowards(
                    transform.position,
                    new Vector3(
                        shadow.shadowPosition.x,
                        transform.position.y,
                        transform.position.z
                    ),
                    moveSpeed * Time.deltaTime
                );
            }
            else
            {
                transform.position = Vector3.MoveTowards(
                    transform.position,
                    new Vector3(
                        transform.position.x,
                        shadow.shadowPosition.y,
                        transform.position.z
                    ),
                    moveSpeed * Time.deltaTime
                );
            }

            if (transform.position == new Vector3(shadow.shadowPosition.x, shadow.shadowPosition.y, transform.position.z))
            {
                isMoving = false;
                shadow.isFalling = false;
                shadow.isClimbing = false;

                animator.StopAllAnimation();
            }

            return;
        }
    }
    public void MoveTo(Vector2Int targetPosition)
    {
        shadow.shadowPosition = targetPosition;

        isMoving = true;
        /*transform.position = new Vector3(
            targetPosition.x,
            targetPosition.y,
            0
        );*/
    }
}
