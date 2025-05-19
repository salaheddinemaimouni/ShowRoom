using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace PJT.CETIM
{
    public class SpatialAnchorManager : MonoBehaviour
    {
        [Header("Prefabs")]
        public List<OVRSpatialAnchor> anchorPrefab = new List<OVRSpatialAnchor>();
        public List<GameObject> placementVisualPrefab = new List<GameObject>();

        public const string NumUuidPlayerPref = "numUuids";

        private Canvas canvas;
        private TextMeshProUGUI uuidText;
        private TextMeshProUGUI savedStatusText;

        private List<OVRSpatialAnchor> anchors = new List<OVRSpatialAnchor>();
        private OVRSpatialAnchor lastCreatedAnchor;

        private AnchorLoader anchorLoader;

        private GameObject visualPreview;
        private bool isPlacing = false;

        private PrefabSelectorManager PrefabSelectrManager;
        [Obsolete]
        private void Awake()
        {
            anchorLoader = GetComponent<AnchorLoader>();
            Debug.Log("✅ AnchorLoader initialized.");
            PrefabSelectrManager = GetComponent<PrefabSelectorManager>();
        }

        [Obsolete]
        private void Update()
        {
            bool triggerHeld = OVRInput.Get(OVRInput.Button.One, OVRInput.Controller.RTouch);
            bool triggerReleased = OVRInput.GetUp(OVRInput.Button.One, OVRInput.Controller.RTouch);

            if (triggerHeld)
            {
                if (!isPlacing)
                    StartPlacingAnchor();

                UpdateAnchorPosition();
            }
            else if (isPlacing && triggerReleased)
            {
                PlaceAnchor();
            }

            if (OVRInput.GetDown(OVRInput.Button.One, OVRInput.Controller.LTouch))
                SaveLastCreatedAnchor();

            if (OVRInput.GetDown(OVRInput.Button.Two, OVRInput.Controller.LTouch))
                UnSaveLastCreatedAnchor();

            if (OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger, OVRInput.Controller.RTouch))
                UnSaveAllAnchors();

            if (OVRInput.GetDown(OVRInput.Button.PrimaryThumbstick, OVRInput.Controller.RTouch))
                StartCoroutine(DelayedLoadSavedAnchors());
        }

        private void StartPlacingAnchor()
        {
            isPlacing = true;
            visualPreview = Instantiate(placementVisualPrefab[PrefabSelectrManager.selectedIndex]);
            Debug.Log("🟡 Started preview mode.");
        }

        private void UpdateAnchorPosition()
        {
            if (visualPreview)
            {
                visualPreview.transform.position = OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch);
                visualPreview.transform.rotation = OVRInput.GetLocalControllerRotation(OVRInput.Controller.RTouch);
                Debug.Log($"👋 Hand position: {visualPreview.transform.position}");
                Debug.Log($"👋 Hand rotation: {visualPreview.transform.rotation}");
            }
        }
        private void PlaceAnchor()
        {
            isPlacing = false;

            Vector3 placePos = visualPreview.transform.position;
            Quaternion placeRot = visualPreview.transform.rotation;

            Destroy(visualPreview);
            Debug.Log("📍 Preview confirmed, placing real anchor.lacePos,placeRot");
            Debug.Log(placePos);
            Debug.Log(placeRot);

            // Vérifie que l'index sélectionné est valide
            if (PrefabSelectrManager.selectedIndex >= 0 && PrefabSelectrManager.selectedIndex < anchorPrefab.Count)
            {
                OVRSpatialAnchor workingAnchor = Instantiate(anchorPrefab[PrefabSelectrManager.selectedIndex], placePos, placeRot);
                Debug.Log($"📍 Placing anchor from selected prefab: {anchorPrefab[PrefabSelectrManager.selectedIndex].name}");

                canvas = workingAnchor.GetComponentInChildren<Canvas>();
                uuidText = canvas.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
                savedStatusText = canvas.transform.GetChild(1).GetComponent<TextMeshProUGUI>();

                uuidText.text = "UUID: ...";
                savedStatusText.text = "Creating...";

                StartCoroutine(AnchorCreated(workingAnchor));
            }
            else
            {
                Debug.LogError("❌ Invalid selectedIndex or no anchor prefab available.");
            }
        }

        private IEnumerator AnchorCreated(OVRSpatialAnchor workingAnchor)
        {
            while (!workingAnchor.Created || !workingAnchor.Localized)
                yield return new WaitForEndOfFrame();

            lastCreatedAnchor = workingAnchor;
            anchors.Add(workingAnchor);

            uuidText.text = "UUID: " + workingAnchor.Uuid;
            savedStatusText.text = "Not Saved";
            Debug.Log($"✅ Anchor created and localized: {workingAnchor.Uuid}");
        }

        [Obsolete]
        private void SaveLastCreatedAnchor()
        {
            if (lastCreatedAnchor == null || !lastCreatedAnchor.Created || !lastCreatedAnchor.Localized)
            {
                Debug.LogWarning("⚠️ No valid anchor to save.");
                return;
            }

            lastCreatedAnchor.Save((anchor, success) =>
            {
                if (success)
                {
                    savedStatusText.text = "Saved";
                    SaveUuidToPlayerPrefs(anchor.Uuid);
                    Debug.Log($"💾 Anchor saved: {anchor.Uuid}");
                }
                else
                {
                    Debug.LogError("❌ Anchor save failed.");
                }
            });
        }

        private void SaveUuidToPlayerPrefs(Guid uuid)
        {
            int count = PlayerPrefs.GetInt(NumUuidPlayerPref, 0);

            for (int i = 0; i < count; i++)
            {
                if (PlayerPrefs.GetString("uuid" + i) == uuid.ToString())
                {
                    Debug.Log($"⚠️ UUID {uuid} already stored.");
                    return;
                }
            }

            PlayerPrefs.SetString("uuid" + count, uuid.ToString());
            PlayerPrefs.SetInt("uuid_index" + count, PrefabSelectrManager.selectedIndex); // 👈 nouvel index sauvegardé
            PlayerPrefs.SetInt(NumUuidPlayerPref, count + 1);
            PlayerPrefs.Save();
            Debug.Log($"✅ UUID {uuid} stored at index {count} with prefabIndex {PrefabSelectrManager.selectedIndex}.");
        }


        [Obsolete]
        private void UnSaveLastCreatedAnchor()
        {
            if (lastCreatedAnchor == null)
            {
                Debug.LogWarning("⚠️ No anchor to erase.");
                return;
            }

            lastCreatedAnchor.Erase((anchor, success) =>
            {
                if (success)
                {
                    savedStatusText.text = "Not Saved";
                    Debug.Log("🧹 Anchor erased.");
                }
                else
                {
                    Debug.LogError("❌ Failed to erase anchor.");
                }
            });
        }

        [Obsolete]
        private void UnSaveAllAnchors()
        {
            foreach (var anchor in anchors)
            {
                anchor.Erase((erasedAnchor, success) =>
                {
                    if (success)
                        Debug.Log($"🧹 Anchor erased: {erasedAnchor.Uuid}");
                });
            }

            anchors.Clear();
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
            Debug.Log("🧼 All anchors and PlayerPrefs cleared.");
        }
        [Obsolete]
        private IEnumerator DelayedLoadSavedAnchors()
        {
            yield return new WaitForSeconds(2f);
            LoadSavedAnchors();
        }
        [Obsolete]
        public void LoadSavedAnchors()
        {
            if (anchorLoader != null)
            {
                anchorLoader.LoadAnchorsByUuid(anchors);
            }
            else
            {
                Debug.LogError("❌ AnchorLoader not assigned.");
            }
        }
    }
}