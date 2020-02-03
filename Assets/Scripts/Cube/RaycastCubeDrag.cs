using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastCubeDrag : MonoBehaviour
{
    private MiniCube _miniCubeSelected;

    public GameObject cube;
    private bool rotate = false;

    // TODO : Fix the bug where you rotate the camera and it doesn't.
    private void Update() {
        if (InputManager.Instance.GetMouseOrTouchBegin()) {
            _RaycastMiniCube();
        }

        if (InputManager.Instance.GetMouseOrTouchEnd()) {
            _TouchEnd();
        }
    }

    private void _RaycastMiniCube() {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        int miniCubeLayer = LayerMask.GetMask("MiniCube");
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, miniCubeLayer)) {

            Debug.Log("Touched mini cube");
            _miniCubeSelected = hit.transform.GetComponent<MiniCube>();
            _miniCubeSelected.selected = true;
        }
    }

    private void _TouchEnd() {
        if (_miniCubeSelected == null)
            return;

        // We get the direction between the mouse and the cube

        var cubeSelectedWorldPos = Camera.main.WorldToScreenPoint(_miniCubeSelected.transform.localPosition);
        var direction = InputManager.Instance.GetMouseTouchPosition() - (cubeSelectedWorldPos);
        direction.Normalize();

        // TODO still a bit clunky, sometime sends both value
        Vector3Int directionInverted = new Vector3Int(Mathf.RoundToInt(direction.y),
            Mathf.RoundToInt(direction.x),
            0);

        Vector2 rowColumn = Vector2.zero;
        // Getting the right row / column when sending the direction
        if (Mathf.Abs(directionInverted.x) == 1) {
            rowColumn.x = Mathf.RoundToInt(_miniCubeSelected.transform.position.x);
        }
        else if (Mathf.Abs(directionInverted.y) == 1) {
            rowColumn.y = Mathf.RoundToInt(_miniCubeSelected.transform.position.y);
        }

        rotate = true;

        CubeManager.Instance.Rotate(rowColumn, directionInverted);
        _miniCubeSelected = null;
        rotate = false;
    }
}