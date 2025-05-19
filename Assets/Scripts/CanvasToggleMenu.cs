using UnityEngine;

public class CanvasToggleMenu : MonoBehaviour
{
    public GameObject canvasPrefab;
    public Transform centerEyeAnchor;
    public float spawnDistance = 1.0f;

    private bool isVisible = false;

    void Start()
    {
        Debug.Log("🚀 CanvasToggleMenu Start() running...");

        if (canvasPrefab == null)
        {
            Debug.LogError("❌ canvasPrefab is NOT assigned in the Inspector.");
        }
        else
        {
            Debug.Log("✅ canvasPrefab is assigned: " + canvasPrefab.name);
            canvasPrefab.SetActive(false);
        }

        if (centerEyeAnchor == null)
        {
            if (Camera.main != null)
            {
                centerEyeAnchor = Camera.main.transform;
                Debug.Log("⚠️ centerEyeAnchor not assigned. Using Camera.main fallback: " + centerEyeAnchor.name);
            }
            else
            {
                Debug.LogError("❌ centerEyeAnchor is NOT assigned, and no Camera.main found.");
            }
        }
        else
        {
            Debug.Log("✅ centerEyeAnchor is assigned: " + centerEyeAnchor.name);
        }
    }

    void Update()
    {
        if (OVRInput.GetDown(OVRInput.RawButton.Start, OVRInput.Controller.LTouch))
        {
            Debug.Log("🎮 Menu button (Left hand) PRESSED.");
            ToggleCanvas();
        }
    }

    private void ToggleCanvas()
    {
        Debug.Log($"🔍 Checking references...\n→ canvasPrefab = {(canvasPrefab ? canvasPrefab.name : "NULL")}\n→ centerEyeAnchor = {(centerEyeAnchor ? centerEyeAnchor.name : "NULL")}");

        if (canvasPrefab == null || centerEyeAnchor == null)
        {
            Debug.LogError("❌ Cannot toggle canvas – one or both required references are still NULL.");
            return;
        }

        isVisible = !isVisible;
        canvasPrefab.SetActive(isVisible);
        Debug.Log($"🔁 Canvas is now {(isVisible ? "VISIBLE ✅" : "HIDDEN ❌")}");

        if (isVisible)
        {
            PositionCanvas();
        }
    }

    private void PositionCanvas()
    {
        if (canvasPrefab == null || centerEyeAnchor == null)
        {
            Debug.LogError("❌ PositionCanvas() failed – missing references.");
            return;
        }

        Vector3 forward = centerEyeAnchor.forward;
        Vector3 targetPos = centerEyeAnchor.position + forward * spawnDistance;

        canvasPrefab.transform.position = targetPos;

        Vector3 lookDirection = (targetPos - centerEyeAnchor.position).normalized;
        Quaternion rotation = Quaternion.LookRotation(lookDirection);
        canvasPrefab.transform.rotation = rotation;

        Debug.Log($"📍 Canvas positioned at {targetPos}, facing direction {lookDirection}");
    }
}
