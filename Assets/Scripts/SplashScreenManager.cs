using UnityEngine;
using UnityEngine.SceneManagement;

public class SplashScreenManager : MonoBehaviour
{
    public float delayBeforeNextScene = 3f; // 3 secondes
    public string nextSceneName = "SampleScene"; // Attention au nom exact !

    private void Start()
    {
        Invoke(nameof(LoadNextScene), delayBeforeNextScene);
    }

    private void LoadNextScene()
    {
        SceneManager.LoadScene(nextSceneName);
    }
}
