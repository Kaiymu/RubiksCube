using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeFace : MonoBehaviour
{
    [SerializeField] float threshHold = 5.0f;
    [SerializeField] float rayLength = 1.0f;

    void Update() {
        UnityEngine.Debug.DrawRay(this.transform.position, this.transform.up * this.rayLength, Color.red);

        UnityEngine.Debug.DrawRay(this.transform.position, -this.transform.up * this.rayLength, Color.magenta);

        UnityEngine.Debug.DrawRay(this.transform.position, this.transform.forward * this.rayLength, Color.blue);

        UnityEngine.Debug.DrawRay(this.transform.position, -this.transform.forward * this.rayLength, Color.cyan);

        UnityEngine.Debug.DrawRay(this.transform.position, this.transform.right * this.rayLength, Color.yellow);

        UnityEngine.Debug.DrawRay(this.transform.position, -this.transform.right * this.rayLength, Color.gray);

        // theta = arcos( a • b / |a| • |b|)
        float upAngle = Mathf.Acos(Vector3.Dot(this.transform.up, Camera.main.transform.forward) / (this.transform.up.magnitude * Camera.main.transform.forward.magnitude));
        upAngle *= 180.0f / Mathf.PI; // In Degrees not radians.

        float downAngle = Mathf.Acos(Vector3.Dot(-this.transform.up, Camera.main.transform.forward) / (this.transform.up.magnitude * Camera.main.transform.forward.magnitude));
        downAngle *= 180.0f / Mathf.PI; // In Degrees not radians.

        float forwardAngle = Mathf.Acos(Vector3.Dot(this.transform.forward, Camera.main.transform.forward) / (this.transform.forward.magnitude * Camera.main.transform.forward.magnitude));
        forwardAngle *= 180.0f / Mathf.PI; // In Degrees not radians.

        float backwardAngle = Mathf.Acos(Vector3.Dot(-this.transform.forward, Camera.main.transform.forward) / (this.transform.forward.magnitude * Camera.main.transform.forward.magnitude));
        backwardAngle *= 180.0f / Mathf.PI; // In Degrees not radians.

        float rightAngle = Mathf.Acos(Vector3.Dot(this.transform.right, Camera.main.transform.forward) / (this.transform.right.magnitude * Camera.main.transform.forward.magnitude));
        rightAngle *= 180.0f / Mathf.PI; // In Degrees not radians.

        float leftAngle = Mathf.Acos(Vector3.Dot(-this.transform.right, Camera.main.transform.forward) / (this.transform.right.magnitude * Camera.main.transform.forward.magnitude));
        leftAngle *= 180.0f / Mathf.PI; // In Degrees not radians.

        if (upAngle < this.threshHold) {
            UnityEngine.Debug.Log("Top Face is facing the Camera");
        }

        if (downAngle < this.threshHold) {
            UnityEngine.Debug.Log("Bottom Face is facing the Camera");
        }

        if (forwardAngle < this.threshHold) {
            UnityEngine.Debug.Log("Forward Face is facing the Camera");
        }

        if (backwardAngle < this.threshHold) {
            UnityEngine.Debug.Log("Backward Face is facing the Camera");
        }

        if (rightAngle < this.threshHold) {
            UnityEngine.Debug.Log("Right Face is facing the Camera");
        }

        if (leftAngle < this.threshHold) {
            UnityEngine.Debug.Log("Left Face is facing the Camera");
        }

        updateFace();
    }

    void updateFace() {
        var x = Mathf.Round(Vector3.Dot(Camera.main.transform.forward, transform.right));
        var y = Mathf.Round(Vector3.Dot(Camera.main.transform.forward, transform.up));
        var z = Mathf.Round(Vector3.Dot(Camera.main.transform.forward, transform.forward));

        Debug.Log(x + " " + y + " " + z); ;
    }
}
