// ✅ VERSION AMÉLIORÉE POUR UN LOADING ROBUSTE ET COHÉRENT AVEC LE TUTO
// Inclut gestion dynamique des prefabs et suppression des UUID invalides
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace PJT.CETIM
{
    public class AnchorLoader : MonoBehaviour
    {
        private SpatialAnchorManager manager;
        private PrefabSelectorManager prefabSelectorManager;
        private Action<OVRSpatialAnchor.UnboundAnchor, bool> onLoadAnchor;
        private HashSet<Guid> alreadyLoaded = new HashSet<Guid>();

        [Obsolete]
        private void Awake()
        {
            manager = GetComponent<SpatialAnchorManager>();
            prefabSelectorManager = GetComponent<PrefabSelectorManager>();
            onLoadAnchor = OnLocalized;
            Debug.Log("🔁 AnchorLoader ready.");
        }

        [Obsolete]
        public void LoadAnchorsByUuid(List<OVRSpatialAnchor> anchors)
        {
            int count = PlayerPrefs.GetInt(SpatialAnchorManager.NumUuidPlayerPref, 0);
            if (count == 0)
            {
                Debug.Log("📭 No UUIDs to load.");
                return;
            }

            var uuidToIndex = new Dictionary<Guid, int>();

            for (int i = 0; i < count; i++)
            {
                string uuidStr = PlayerPrefs.GetString("uuid" + i);
                int prefabIndex = PlayerPrefs.GetInt("uuid_index" + i, 0);

                if (Guid.TryParse(uuidStr, out Guid uuid))
                {
                    if (!alreadyLoaded.Contains(uuid))
                    {
                        uuidToIndex[uuid] = prefabIndex;
                        alreadyLoaded.Add(uuid);
                    }
                }
            }

            if (uuidToIndex.Count == 0)
            {
                Debug.Log("✅ All anchors already loaded.");
                return;
            }

            Debug.Log($"🚀 Requesting load of {uuidToIndex.Count} anchors...");
            LoadWithRetry(uuidToIndex);
        }

        [Obsolete]
        private void LoadWithRetry(Dictionary<Guid, int> uuidToIndex)
        {
            StartCoroutine(LoadCoroutine(uuidToIndex));
        }

        [Obsolete]
        private IEnumerator LoadCoroutine(Dictionary<Guid, int> uuidToIndex)
        {
            int retries = 3;
            float delay = 2f;

            for (int attempt = 1; attempt <= retries; attempt++)
            {
                Debug.Log($"🔁 Attempt {attempt} to load anchors...");

                bool completed = false;
                bool gotAnchors = false;

                OVRSpatialAnchor.LoadUnboundAnchors(new OVRSpatialAnchor.LoadOptions
                {
                    Timeout = 0,
                    StorageLocation = OVRSpace.StorageLocation.Local,
                    Uuids = new List<Guid>(uuidToIndex.Keys).ToArray()
                }, anchors =>
                {
                    if (anchors != null && anchors.Length > 0)
                    {
                        Debug.Log($"📦 Loaded {anchors.Length} anchors.");
                        gotAnchors = true;

                        foreach (var anchor in anchors)
                        {
                            anchor.Localize((unboundAnchor, success) =>
                            {
                                if (success)
                                {
                                    int prefabIndex = uuidToIndex[unboundAnchor.Uuid];
                                    OnLocalizedWithPrefabIndex(unboundAnchor, prefabIndex);
                                }
                                else
                                {
                                    Debug.LogWarning("⚠️ Failed to localize anchor. Removing from PlayerPrefs.");
                                    RemoveUuidFromPrefs(unboundAnchor.Uuid);
                                }
                            });
                        }
                    }
                    else
                    {
                        Debug.LogWarning("🚫 No anchors returned.");
                    }

                    completed = true;
                });

                yield return new WaitUntil(() => completed);

                if (gotAnchors)
                    yield break;

                yield return new WaitForSeconds(delay);
            }

            Debug.LogError("❌ All retry attempts failed.");
        }


        [Obsolete]
        private void OnLocalized(OVRSpatialAnchor.UnboundAnchor unboundAnchor, bool success)
        {
            if (!success)
            {
                Debug.LogWarning("⚠️ Failed to localize anchor. Removing from PlayerPrefs.");
                RemoveUuidFromPrefs(unboundAnchor.Uuid);
                return;
            }

            var pose = unboundAnchor.Pose;
            int index = prefabSelectorManager.selectedIndex;
            var prefab = manager.anchorPrefab[Mathf.Clamp(index, 0, manager.anchorPrefab.Count - 1)];
            var spatialAnchor = Instantiate(prefab, pose.position, pose.rotation);
            unboundAnchor.BindTo(spatialAnchor);

            var texts = spatialAnchor.GetComponentsInChildren<TextMeshProUGUI>();
            if (texts.Length >= 2)
            {
                texts[0].text = "UUID: " + spatialAnchor.Uuid.ToString();
                texts[1].text = "Loaded from Device";
            }

            Debug.Log($"🎯 Anchor loaded and placed: {spatialAnchor.Uuid}");
        }

        private void RemoveUuidFromPrefs(Guid uuid)
        {
            int count = PlayerPrefs.GetInt(SpatialAnchorManager.NumUuidPlayerPref, 0);
            List<string> validUuids = new();
            for (int i = 0; i < count; i++)
            {
                string id = PlayerPrefs.GetString("uuid" + i);
                if (id != uuid.ToString())
                    validUuids.Add(id);
            }

            PlayerPrefs.DeleteAll();
            for (int i = 0; i < validUuids.Count; i++)
                PlayerPrefs.SetString("uuid" + i, validUuids[i]);

            PlayerPrefs.SetInt(SpatialAnchorManager.NumUuidPlayerPref, validUuids.Count);
            PlayerPrefs.Save();

            Debug.Log($"🗑️ UUID {uuid} removed from PlayerPrefs.");
        }

        [Obsolete]
        private void OnLocalizedWithPrefabIndex(OVRSpatialAnchor.UnboundAnchor unboundAnchor, int index)
        {
            var pose = unboundAnchor.Pose;

            var prefab = manager.anchorPrefab[Mathf.Clamp(index, 0, manager.anchorPrefab.Count - 1)];
            var spatialAnchor = Instantiate(prefab, pose.position, pose.rotation);
            unboundAnchor.BindTo(spatialAnchor);

            var texts = spatialAnchor.GetComponentsInChildren<TextMeshProUGUI>();
            if (texts.Length >= 2)
            {
                texts[0].text = "UUID: " + spatialAnchor.Uuid.ToString();
                texts[1].text = "Loaded from Device";
            }

            Debug.Log($"🎯 Anchor loaded and placed (index {index}): {spatialAnchor.Uuid}");
        }


    }
}
