using UnityEngine;
using UnityEngine.SceneManagement;

namespace PJT.CETIM
{
    public class SceneManagerCustom : MonoBehaviour
    {
        public void LoadSceneByName(string sceneName)
        {
            Debug.Log($"📦 Changement de scène vers : {sceneName}");
            SceneManager.LoadScene(sceneName);
        }
    }
}
