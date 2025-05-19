using System.IO;
using UnityEngine;

public class LogManager : MonoBehaviour
{
    public static LogManager Instance;
    private string logBuffer = "";
    private int runIndex;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Load number of previous runs
            runIndex = PlayerPrefs.GetInt("logRunIndex", 0) + 1;
            PlayerPrefs.SetInt("logRunIndex", runIndex);
            PlayerPrefs.Save();

            Application.quitting += SaveLogToFile;
            Debug.Log("📝 LogManager initialized. This is run #" + runIndex);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Log(string message)
    {
        Debug.Log(message);
        logBuffer += message + "\n";
    }

    private void SaveLogToFile()
    {
        string path = Path.Combine(Application.persistentDataPath, $"text{runIndex}.txt");

        try
        {
            File.WriteAllText(path, logBuffer);
            Debug.Log("📁 Log file saved at: " + path);
        }
        catch (System.Exception e)
        {
            Debug.LogError("❌ Failed to save log: " + e.Message);
        }
    }
}
