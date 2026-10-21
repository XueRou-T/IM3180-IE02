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

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (string.IsNullOrEmpty(nextSpawnPointName)) return;

        GameObject spawnPoint = GameObject.Find(nextSpawnPointName);
        if (spawnPoint != null)
        {
            transform.position = spawnPoint.transform.position;
        }

        nextSpawnPointName = ""; 
    }
}