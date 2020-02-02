using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastCubeDrag : MonoBehaviour
{
    private MiniCube _miniCubeSelected;
    private Vector3 _previousMousePos = Vector3.zero;

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
                _previousMousePos = Input.mousePosition;
            }
        }
    }

    private void _UnselectCube() {
        if (Input.GetMouseButtonUp(0) || TouchEndMobile()) {
            _miniCubeSelected = null;
            rotate = false;
            _previousMousePos = Vector3.zero;
        }
    }

    // Need to be changed, and relative to the camera / position of the cube
    private void _DirectionDrag() {
        if (rotate)
            return;

        if ((Input.GetMouseButton(0) || Input.touchCount > 0) && _miniCubeSelected != null) {
            Vector3 inputSubstracted = MouseOrMobilePosition() - _previousMousePos;

            var direction = inputSubstracted - _miniCubeSelected.transform.position;

            // TODO Use the difference between player pos / mouse pos to make the right rotation and not hardcoded like so
            direction.Normalize();

            if (Mathf.Abs(inputSubstracted.x) > Mathf.Abs(inputSubstracted.y)) {
                int miniCubeYPos = Mathf.RoundToInt(_miniCubeSelected.transform.position.y);
                rotate = true;

                if (_previousMousePos.x < MouseOrMobilePosition().x) {
                    CubeManager.Instance.RotateHorizontal(miniCubeYPos, Vector3.down);
                }
                else if (_previousMousePos.x > MouseOrMobilePosition().x) {
                    CubeManager.Instance.RotateHorizontal(miniCubeYPos, Vector3.up);
                }
            }

            if (Mathf.Abs(inputSubstracted.x) < Mathf.Abs(inputSubstracted.y)) {
                int miniCubeXPos = Mathf.RoundToInt(_miniCubeSelected.transform.position.x);
                rotate = true;
                if (_previousMousePos.y < MouseOrMobilePosition().y) {
                    CubeManager.Instance.RotateVertical(miniCubeXPos, Vector3.right);
                }
                else if (_previousMousePos.y > MouseOrMobilePosition().y) {
                    CubeManager.Instance.RotateVertical(miniCubeXPos, Vector3.left);
                }
            }

            _previousMousePos = MouseOrMobilePosition();
        }
    }

    public Vector3 MouseOrMobilePosition() {


#if UNITY_EDITOR
        return Input.mousePosition;
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
