
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float smoothSpeed = 5f;

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

        Vector3 targetPosition = target.position;
        targetPosition.z = -10f;

        float halfHeight = cam.orthographicSize;
        float halfWidth = halfHeight * cam.aspect;

        targetPosition.x = Mathf.Clamp(
            targetPosition.x,
            mapMin.x + halfWidth,
            mapMax.x - halfWidth
        );

        targetPosition.y = Mathf.Clamp(
            targetPosition.y,
            mapMin.y + halfHeight,
            mapMax.y - halfHeight
        );

        transform.position = Vector3.Lerp(
            transform.position,
            targetPosition,
            smoothSpeed * Time.deltaTime
        );
    }
}
