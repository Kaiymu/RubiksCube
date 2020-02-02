using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastCubeDrag : MonoBehaviour
{
    private MiniCube _miniCubeSelected;
    private Vector3 _previousMousePos;

    // TODO : Refactor with Input.Touch, for now we'll use editor for our test
    private void Update() {
        _RaycastMiniCube();
        _UnselectCube();
        _DirectionDrag();
    }

    private void _RaycastMiniCube() {
        if (Input.GetMouseButtonDown(0)) {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            int miniCubeLayer = LayerMask.GetMask("MiniCube");
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, miniCubeLayer)) {

                Debug.Log("Touched mini cube");
                _miniCubeSelected = hit.transform.GetComponent<MiniCube>();
                _miniCubeSelected.selected = true;
            }
        }
    }

    private void _UnselectCube() {
        if (Input.GetMouseButtonUp(0)) {
            _miniCubeSelected = null;
        }
    }

    private void _DirectionDrag() {
        if (Input.GetMouseButton(0) && _miniCubeSelected != null) {
            if (_previousMousePos.x < Input.mousePosition.x) {
                Debug.LogError(_miniCubeSelected.xyz.y);
                CubeManager.Instance._GetSameHorizontalCubes(_miniCubeSelected.xyz.y);
                Debug.LogError("Go right"); // Make to rotation happen here
            }
            else if (_previousMousePos.x > Input.mousePosition.x) {
                Debug.LogError("Go left");
            }

            if (_previousMousePos.y < Input.mousePosition.y) {
                Debug.LogError("Go top");
            }
            else if (_previousMousePos.y > Input.mousePosition.y) {
                Debug.LogError("Go bottom");
            }

            _previousMousePos = Input.mousePosition;
        }
    }
}
