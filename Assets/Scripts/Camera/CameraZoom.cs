using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraZoom : MonoBehaviour
{
    [Header("Boundaries min and max for camera, x is min, y max")]
    public Vector2 boundariesZoom = new Vector2(5, 10);

    public float zoomSpeed;

    private Transform _lookAt;

    private void Start() {
        _lookAt = GameManager.Instance.CenterOfCube;
        var distanceLookAt = Vector3.Distance(_lookAt.transform.position, transform.position);

        boundariesZoom.x = Mathf.Abs(distanceLookAt) - boundariesZoom.x;
        boundariesZoom.y = Mathf.Abs(distanceLookAt) + boundariesZoom.y;
    }

    private void Update() {
        float scrollOrMobileValue = InputManager.Instance.GetScrollOrMobileZoom();

        if (scrollOrMobileValue != 0f) {
            Vector3 zoomInOut = transform.position;
            // Set back the previous pos when trying to reach out the boundaries
            zoomInOut = new Vector3(zoomInOut.x, zoomInOut.y, zoomInOut.z + (scrollOrMobileValue * zoomSpeed));

            var distanceLookAt = Vector3.Distance(_lookAt.transform.position, zoomInOut);

            // Zoom in / out the camera if the value doesn't outreach the distance limit
            if (Mathf.Abs(distanceLookAt) > boundariesZoom.x && Mathf.Abs(distanceLookAt) < boundariesZoom.y) {
                transform.position = zoomInOut;
            }
        }
    }

    private void LateUpdate() {
        transform.LookAt(_lookAt);
    }
}
