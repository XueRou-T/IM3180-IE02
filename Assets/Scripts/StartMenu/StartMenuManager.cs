using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class StartMenuManager : MonoBehaviour
{
    [Header("Scene")]
    public string gameSceneName;

    public void PlayGame()
    {
        Debug.Log("Play Game");

        PlayerPrefs.SetInt("TutorialCompleted", 0);
        PlayerPrefs.DeleteKey("ReturnPuzzleScene");
        PlayerPrefs.SetFloat("PlayerSaturation", 0f);
        PlayerPrefs.Save();

        // Tell PlayerPersistence to spawn the player near the machine for a new game
        PlayerPersistence.nextSpawnPointName = "SpawnNearMachine";

        SceneManager.LoadScene(gameSceneName);
    }

    public void QuitGame()
    {
        Debug.Log("QuitGame");
        Application.Quit();
    }
}