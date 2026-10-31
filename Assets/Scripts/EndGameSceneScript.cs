using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGameSceneScript : MonoBehaviour
{
    public string nextSceneName;

    public void GoToNextScene()
    {
        SceneManager.LoadScene(nextSceneName);
    }
}
