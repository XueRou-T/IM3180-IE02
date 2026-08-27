using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private float gridSize = 1f;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            GoLeft();
            Debug.Log("Player went left");
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            GoRight();
            Debug.Log("Player went right");
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            GoUp();
            Debug.Log("Player went up");
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            GoDown();
            Debug.Log("Player went down");
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            JumpLeft();
            Debug.Log("Player jump left");
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            JumpRight();
            Debug.Log("Player jump right");
        }
    }

    void GoLeft()
    {
        transform.position += Vector3.left * gridSize;
    }

    void GoRight()
    {
        transform.position += Vector3.right * gridSize;
    }

    void GoUp()
    {
        transform.position += Vector3.up * gridSize;
    }

    void GoDown()
    {
        transform.position += Vector3.down * gridSize;
    }

    void JumpLeft()
    {
        transform.position += new Vector3(-1, 1, 0) * gridSize;
    }

    void JumpRight()
    {
        transform.position += new Vector3(1, 1, 0) * gridSize;
    }

}
