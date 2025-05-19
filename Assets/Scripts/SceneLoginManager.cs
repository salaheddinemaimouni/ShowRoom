using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoginManager : MonoBehaviour
{
    public TMP_InputField passwordInputField;
    public TextMeshProUGUI feedbackText;
    public GameObject loginCanvas;
    public Transform centerEyeAnchor;  // Pour positionner Canvas devant
    public float spawnDistance = 1.5f;

    public static bool isAdmin = false;

    private string adminPassword = "z3ma";

    private void Start()
    {
        if (passwordInputField == null || feedbackText == null || loginCanvas == null || centerEyeAnchor == null)
        {
            Debug.LogError("❌ SceneLoginManager: Missing references (InputField, Text, Canvas, CenterEyeAnchor) !");
            return;
        }

        feedbackText.text = "";
        PositionCanvasInFrontOfUser();
    }

    private void PositionCanvasInFrontOfUser()
    {
        Vector3 forward = centerEyeAnchor.forward;
        Vector3 spawnPos = centerEyeAnchor.position + forward * spawnDistance;

        loginCanvas.transform.position = spawnPos;
        loginCanvas.transform.rotation = Quaternion.LookRotation(spawnPos - centerEyeAnchor.position);

        Debug.Log($"📍 Login Canvas positioned at {spawnPos}");
    }

    public void CheckLogin()
    {
        string enteredPassword = passwordInputField.text.Trim();

        if (enteredPassword == adminPassword)
        {
            isAdmin = true;
            feedbackText.text = "<color=green>Bienvenue Admin 👑</color>";
            Debug.Log("✅ Login Admin accepté.");

            Invoke(nameof(EnterApplication), 1.5f);
        }
        else
        {
            feedbackText.text = "<color=red>Code Incorrect ❌</color>";
            Debug.Log("❌ Code incorrect. Attente décision utilisateur.");
        }
    }

    public void ContinueAsUser()
    {
        isAdmin = false;
        feedbackText.text = "<color=orange>Mode Utilisateur Normal 🧑‍💻</color>";
        Debug.Log("➡️ Continuer en mode utilisateur normal.");

        Invoke(nameof(EnterApplication), 1.5f);
    }

    private void EnterApplication()
    {
        loginCanvas.SetActive(false);
    }
    
}
