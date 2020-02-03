using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public Transform lookAt;

    private void Awake() {
        
    }

    private void Update() {
        transform.RotateAround(lookAt.transform.position, Vector3.forward, 500 * Time.deltaTime);
    }
}
