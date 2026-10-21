using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class ExplosionManager : MonoBehaviour
{
    [Header("Timer Settings")]
    public float delayTime = 3f; // How many seconds to wait

    [Header("Next Scene")]
    public string nextSceneName = "Lab";

    void Start()
    {
        // Start the countdown as soon as the scene opens
        StartCoroutine(WaitAndLoadNextScene());
    }

    IEnumerator WaitAndLoadNextScene()
    {
        // Wait for the specified number of seconds
        yield return new WaitForSeconds(delayTime);

        // Load the lab scene
        SceneManager.LoadScene(nextSceneName);
    }
}