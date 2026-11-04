using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerPersistence : MonoBehaviour
{
    public static PlayerPersistence Instance;
    public static string nextSpawnPointName = "";

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"[PlayerPersistence] Scene loaded: {scene.name} | Target spawn requested: '{nextSpawnPointName}'");

        if (string.IsNullOrEmpty(nextSpawnPointName)) return;

        // Note: GameObject.Find() only works on ACTIVE objects in the scene hierarchy
        GameObject spawnPoint = GameObject.Find(nextSpawnPointName);
        if (spawnPoint != null)
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                player.transform.position = spawnPoint.transform.position;
                Debug.Log($"[PlayerPersistence] Successfully moved player to {spawnPoint.name}");
            }
            else
            {
                Debug.LogWarning("[PlayerPersistence] Spawn point found, but could not find any GameObject with the tag 'Player' in this scene!");
            }
        }
        else
        {
            Debug.LogWarning($"[PlayerPersistence] Could not find any active GameObject named '{nextSpawnPointName}' in the scene.");
        }

        nextSpawnPointName = "";
    }
}