
using UnityEngine;

public class L02_CameraFollow : MonoBehaviour
{
    public Transform target;
    public float smoothSpeed = 5f;

    [Header("Map Boundaries")]
    public Vector2 mapMin;
    public Vector2 mapMax;

    private Camera cam;

    private void Start()
    {
        cam = GetComponent<Camera>();
    }

    private void LateUpdate()
    {
        if (target == null || cam == null)
            return;

        float halfHeight = cam.orthographicSize;
        float halfWidth = halfHeight * cam.aspect;

        float minX = mapMin.x + halfWidth;
        float maxX = mapMax.x - halfWidth;
        float minY = mapMin.y + halfHeight;
        float maxY = mapMax.y - halfHeight;

        Vector3 targetPosition = target.position;

        // Keep the camera inside the map.
        targetPosition.x = minX > maxX
            ? (mapMin.x + mapMax.x) / 2f
            : Mathf.Clamp(targetPosition.x, minX, maxX);

        targetPosition.y = minY > maxY
            ? (mapMin.y + mapMax.y) / 2f
            : Mathf.Clamp(targetPosition.y, minY, maxY);

        targetPosition.z = -10f;

        transform.position = Vector3.Lerp(
            transform.position,
            targetPosition,
            smoothSpeed * Time.deltaTime
        );
    }
}
