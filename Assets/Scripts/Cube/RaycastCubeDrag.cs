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

    private void _DirectionDrag() {
        if (rotate)
            return;

        if ((Input.GetMouseButton(0) || Input.touchCount > 0) && _miniCubeSelected != null) {
            Vector3 t = MouseOrMobilePosition() - _previousMousePos;

            if (Mathf.Abs(t.x) > Mathf.Abs(t.y)) {
                int miniCubeYPos = Mathf.RoundToInt(_miniCubeSelected.transform.position.y);
                rotate = true;
                if (_previousMousePos.x < MouseOrMobilePosition().x) {
                    CubeManager.Instance.RotateHorizontal(miniCubeYPos, Vector2.down);
                }
                else if (_previousMousePos.x > MouseOrMobilePosition().x) {
                    CubeManager.Instance.RotateHorizontal(miniCubeYPos, Vector2.up);
                }
            }

            if (Mathf.Abs(t.x) < Mathf.Abs(t.y)) {
                int miniCubeXPos = Mathf.RoundToInt(_miniCubeSelected.transform.position.x);
                rotate = true;
                if (_previousMousePos.y < MouseOrMobilePosition().y) {
                    CubeManager.Instance.RotateVertical(miniCubeXPos, Vector2.right);
                }
                else if (_previousMousePos.y > MouseOrMobilePosition().y) {
                    CubeManager.Instance.RotateVertical(miniCubeXPos, Vector2.left);
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
