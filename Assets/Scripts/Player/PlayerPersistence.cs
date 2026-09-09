using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerPersistence : MonoBehaviour
{
    public static PlayerPersistence Instance;
    public static string nextSpawnPointName = ""; // set by SceneDoor before loading a scene

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

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // If no spawn point was requested, leave the player wherever it already is
        // (this covers the very first time you press Play in the Lab scene)
        if (string.IsNullOrEmpty(nextSpawnPointName)) return;

        GameObject spawnPoint = GameObject.Find(nextSpawnPointName);
        if (spawnPoint != null)
        {
            transform.position = spawnPoint.transform.position;
        }

        nextSpawnPointName = ""; // reset so it doesn't affect future scene loads accidentally
    }
}