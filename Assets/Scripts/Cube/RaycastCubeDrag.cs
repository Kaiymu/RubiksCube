using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastCubeDrag : MonoBehaviour
{
    private MiniCube _miniCubeSelected;
    private Vector3 _previousMousePos = Vector3.zero;

    public GameObject cube;
    private bool rotate = false;

    // TODO : Refactor with Input.Touch, for now we'll use editor for our test
    // TODO : Create an InputManager to also handle futur camera zoom in / out and movement
    // TODO : Fix the bug where you rotate the camera and it doesn't s
    private void Update() {
        _RaycastMiniCube();
        _UnselectCube();
        _DirectionDrag();
    }

    private void _RaycastMiniCube() {
        if (Input.GetMouseButtonDown(0) || TouchBeginMobile()) {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            int miniCubeLayer = LayerMask.GetMask("MiniCube");
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, miniCubeLayer)) {

                Debug.Log("Touched mini cube");
                _miniCubeSelected = hit.transform.GetComponent<MiniCube>();
                _miniCubeSelected.selected = true;
                var v3 = Input.mousePosition;
                v3.z = 10.0f;
                _previousMousePos = Camera.main.ScreenToWorldPoint(v3);
            }
        }
    }

    private void _UnselectCube() {
        if (_miniCubeSelected == null)
            return;

        if (Input.GetMouseButtonUp(0) || TouchEndMobile()) {
            var mousePosition = MouseOrMobilePosition();

            // We get the direction between the mouse and the cube

            var cubeSelectedWorldPos = Camera.main.WorldToScreenPoint(_miniCubeSelected.transform.localPosition);
            var direction = MouseOrMobilePosition() - (cubeSelectedWorldPos);
            direction.Normalize();

            // TODO still a bit clunky, sometime sends both value
            Vector3Int directionInverted = new Vector3Int(Mathf.RoundToInt(direction.y),
                Mathf.RoundToInt(direction.x),
                0);

            Vector2 rowColumn = Vector2.zero;
            // Getting the right row / column when sending the direction
            if (Mathf.Abs(directionInverted.x) == 1) {
                rowColumn.x = Mathf.RoundToInt(_miniCubeSelected.transform.position.x);
            } else if (Mathf.Abs(directionInverted.y) == 1) {
                rowColumn.y = Mathf.RoundToInt(_miniCubeSelected.transform.position.y);
            }

            rotate = true;

            CubeManager.Instance.Rotate(rowColumn, directionInverted);
            //_miniCubeSelected = null;
            rotate = false;
            _previousMousePos = Vector3.zero;
        }
    }

    // Need to be changed, and relative to the camera / position of the cube
    private void _DirectionDrag() {
        if (rotate)
            return;

        if ((Input.GetMouseButton(0) || Input.touchCount > 0) && _miniCubeSelected != null) {
            _previousMousePos = MouseOrMobilePosition();
        }
    }

    public Vector3 MouseOrMobilePosition() {


#if UNITY_EDITOR
        var mousePosition = Input.mousePosition;
        mousePosition.z = 10.0f;
        return mousePosition;
#elif UNITY_IPHONE || UNITY_ANDROID
        if (Input.touchCount > 0) {
                   return Input.GetTouch(0).position;
        }
#endif
        return Vector3.zero;
    }

    public bool TouchBeginMobile() {
        foreach (Touch touch in Input.touches) {
            return (touch.phase == TouchPhase.Began);
        }
        return false;
    }

    public bool TouchEndMobile() {
        foreach (Touch touch in Input.touches) {
            return (touch.phase == TouchPhase.Ended);
        }
        return false;
    }
}