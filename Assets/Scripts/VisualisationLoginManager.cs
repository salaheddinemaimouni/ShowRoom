using UnityEngine;
using UnityEngine.SceneManagement;

public class VisualizerLoginManager : MonoBehaviour
{
    public void OnVisualizerButtonClick()
    {
        Debug.Log("Visualizer button clicked. Loading visualizer scene...");
        SceneManager.LoadScene("VisualizerScene"); // Replace with the actual name of your Visualizer Scene
    }
}
