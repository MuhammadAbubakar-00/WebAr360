using UnityEngine;
using System.Collections;

public class AbsoluteWorldLock : MonoBehaviour
{
    public Transform arCamera;
    public GameObject[] pictures = new GameObject[8];
    public float radius = 3f;

    private Vector3[] fixedPositions = new Vector3[8];
    private Quaternion[] fixedRotations = new Quaternion[8];
    private bool coordinatesCalculated = false;

    IEnumerator Start()
    {
        if (arCamera == null) arCamera = Camera.main.transform;

        // Wait a split second for WebAR tracking to stabilize its initial origin point
        yield return new WaitForSeconds(0.5f);

        Vector3 centerPosition = arCamera.position;

        // Calculate and save the strict absolute spots in the world room
        for (int i = 0; i < 8; i++)
        {
            if (pictures[i] == null) continue;

            float angleRadians = (i * 45f) * Mathf.Deg2Rad;
            float x = centerPosition.x + Mathf.Sin(angleRadians) * radius;
            float z = centerPosition.z + Mathf.Cos(angleRadians) * radius;
            float y = centerPosition.y; // Eye-level

            fixedPositions[i] = new Vector3(x, y, z);

            Vector3 directionToCenter = centerPosition - fixedPositions[i];
            directionToCenter.y = 0; // Keep upright
            fixedRotations[i] = Quaternion.LookRotation(directionToCenter);
        }

        coordinatesCalculated = true;
    }

    // LateUpdate runs AFTER the WebAR SDK updates positions every frame
    void LateUpdate()
    {
        if (!coordinatesCalculated) return;

        // Forcefully override whatever the SDK tracking layer did this frame
        for (int i = 0; i < 8; i++)
        {
            if (pictures[i] == null) continue;

            // This overrides parent movement completely
            pictures[i].transform.position = fixedPositions[i];
            pictures[i].transform.rotation = fixedRotations[i];
        }
    }
}