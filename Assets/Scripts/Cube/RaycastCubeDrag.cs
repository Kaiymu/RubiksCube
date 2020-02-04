using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RaycastCubeDrag : MonoBehaviour
{
    private void Update() {
        if (InputManager.Instance.GetMouseOrTouchBegin()) {
            _TryRaycastingMiniCube();
        }
    }

    ///<summary>
    /// We fire a raycast from the input (Mouse / Touch) position, if we touch a MiniCube, we store it.
    ///</summary>
    private void _TryRaycastingMiniCube() {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        int miniCubeLayer = LayerMask.GetMask("MiniCube");
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, miniCubeLayer)) {

            CubeManager.Instance.MiniCubeSelected = hit.transform.GetComponent<MiniCube>();
        }
    }
}