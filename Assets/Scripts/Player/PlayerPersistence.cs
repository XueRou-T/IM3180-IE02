using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.VFX;

public class PlayerPersistence : MonoBehaviour
{
    public static PlayerPersistence Instance;
    public static string nextSpawnPointName = "";
    private OverWorldSFX overWorldSFX;
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
        if(scene.name == "Puzzle1" || scene.name == "Puzzle2" || scene.name == "TutorialMinigame")
        {
            gameObject.SetActive(false);
        }
        else
        {
            gameObject.SetActive(true);
        }

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
                overWorldSFX = GameObject.FindGameObjectWithTag("OverWorldSFX").GetComponent<OverWorldSFX>();
                if (overWorldSFX != null) player.GetComponent<PlayerMovement>().sfxManager = overWorldSFX;
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