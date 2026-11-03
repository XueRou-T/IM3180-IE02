using UnityEngine;

public class PlayerSpawnerExplore : MonoBehaviour
{
    [Header("Name of this Spawn Point")]
    public string spawnPointName = "SpawnNearDoor"; // Change this in the Inspector for the machine spawn point to "SpawnNearMachine"

    void Start()
    {
        // Get the target spawn name requested by the game (default to "SpawnNearDoor" if empty)
        string targetSpawn = PlayerPersistence.nextSpawnPointName;
        if (string.IsNullOrEmpty(targetSpawn))
        {
            targetSpawn = "SpawnNearDoor";
        }

        // If this specific spawn point is NOT the one requested, do nothing and let the correct spawn point handle it
        if (spawnPointName != targetSpawn)
        {
            return;
        }

        // Find the player in the new scene
        GameObject player = GameObject.FindWithTag("Player");

        if (player != null)
        {
            // Move the player to this spawner's position
            player.transform.position = transform.position;
        }
    }
}