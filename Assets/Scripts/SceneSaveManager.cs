using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PJT.CETIM
{
    public class SceneSaveManager : MonoBehaviour
    {
        [Header("UI")]
        public RectTransform canvasSceneListParent;
        public GameObject buttonPrefabScene;
        public TMP_InputField sceneNameInputField;
        
        private const string saveFolder = "SavedScenes";
        private string fullSavePath => Path.Combine(Application.persistentDataPath, saveFolder);

        private void Start()
        {
            if (!Directory.Exists(fullSavePath))
                Directory.CreateDirectory(fullSavePath);

            LoadAllSceneButtons();
        }
        
        public void SaveCurrentScene()
        {
            string sceneName = sceneNameInputField != null && !string.IsNullOrWhiteSpace(sceneNameInputField.text)
                ? sceneNameInputField.text
                : "Scene_" + System.DateTime.Now.ToString("yyyyMMdd_HHmmss");

            var saveData = new SceneSaveData { sceneName = sceneName };

            foreach (var anchor in FindObjectsByType<AnchorPlaced>(FindObjectsSortMode.None))
            {
                saveData.anchors.Add(new AnchorSaveData
                {
                    prefabName = anchor.prefabName,
                    position = anchor.transform.position,
                    rotation = anchor.transform.rotation
                });
            }

            string json = JsonUtility.ToJson(saveData, true);
            string path = Path.Combine(fullSavePath, sceneName + ".json");
            File.WriteAllText(path, json);

            Debug.Log($"💾 Scene saved to {path}");

            CreateButtonForScene(sceneName);
        }

        public void LoadScene(string sceneName)
        {
            Debug.Log("Loading scene: " + sceneName);

            // Charger fichier JSON
            string path = Path.Combine(Application.persistentDataPath, sceneName + ".json");

            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                SceneSaveData saveData = JsonUtility.FromJson<SceneSaveData>(json);

                // Replacer tous les prefabs enregistrés
                foreach (var anchorData in saveData.anchors)
                {
                    GameObject prefab = FindPrefabByName(anchorData.prefabName);
                    if (prefab != null)
                    {
                        Instantiate(prefab, anchorData.position, anchorData.rotation);
                    }
                    else
                    {
                        Debug.LogWarning($"⚠️ Prefab '{anchorData.prefabName}' introuvable !");
                    }
                }
            }
            else
            {
                Debug.LogError("❌ Fichier de sauvegarde non trouvé : " + path);
            }
        }

        private void LoadAllSceneButtons()
        {
            foreach (var file in Directory.GetFiles(fullSavePath, "*.json"))
            {
                string sceneName = Path.GetFileNameWithoutExtension(file);
                CreateButtonForScene(sceneName);
            }
        }

        private void CreateButtonForScene(string sceneName)
        {
            GameObject buttonGO = Instantiate(buttonPrefabScene, canvasSceneListParent);
            buttonGO.GetComponentInChildren<TextMeshProUGUI>().text = sceneName;
            buttonGO.GetComponent<Button>().onClick.AddListener(() => LoadScene(sceneName));
        }
        private GameObject FindPrefabByName(string prefabName)
        {
            foreach (var prefab in PrefabSelectorManager.Instance.prefabsToPlace)
            {
                if (prefab.name == prefabName)
                    return prefab;
            }
            return null;
        }


        
    }

}
