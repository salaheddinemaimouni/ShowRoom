using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class AdminLoginManager : MonoBehaviour
{
    public TMP_InputField passwordInput;
    public string adminPassword = "0000";

    public void CheckPassword()
    {
        Debug.Log("🔑 Bouton 'Connexion Admin' cliqué.");
        if (passwordInput.text == adminPassword)
        {
            Debug.Log("✅ Mot de passe correct. Chargement Scene Admin...");
            SceneManager.LoadScene("SceneAdmin");  // ID 1 = Scene Admin (défini dans Build Settings)
        }
        else
        {
            Debug.LogWarning("❌ Mot de passe incorrect.");
            passwordInput.text = "";
            passwordInput.placeholder.GetComponent<TMP_Text>().text = "Code incorrect!";
        }
    }
}
