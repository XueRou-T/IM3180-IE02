using UnityEngine;

public class PlayerSpawnerExplore : MonoBehaviour
{
    void Start()
    {
        // Find the player in the new scene
        GameObject player = GameObject.FindWithTag("Player");

        if (player != null)
        {
            // Move the player to this spawner's position
            player.transform.position = transform.position;
        }
    }
}