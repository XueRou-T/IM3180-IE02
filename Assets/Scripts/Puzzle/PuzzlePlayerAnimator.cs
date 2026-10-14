using UnityEngine;

public class PuzzlePlayerAnimator : MonoBehaviour
{
    public PuzzlePlayer player;
    public Animator animator;

    public void MoveRight()
    {
        animator.SetBool("isClimbing", false);
        animator.SetBool("isFalling", false);
        animator.SetInteger("isMoving", 1);
    }

    public void MoveLeft()
    {
        animator.SetBool("isClimbing", false);
        animator.SetBool("isFalling", false);
        animator.SetInteger("isMoving", -1);
    }

    public void MoveUp()
    {
        animator.SetBool("isClimbing", true);
        animator.SetBool("isFalling", false);
        animator.SetInteger("isMoving", 0);
    }

    public void MoveDown()
    {
        animator.SetBool("isClimbing", true);
        animator.SetBool("isFalling", false);
        animator.SetInteger("isMoving", 0);
    }

    public void StopAllAnimation()
    {
        animator.SetBool("isClimbing", false);
        animator.SetBool("isFalling", false);
        animator.SetInteger("isMoving", 0);
    }
}
