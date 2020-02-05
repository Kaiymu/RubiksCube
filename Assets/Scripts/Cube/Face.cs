using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Face : MonoBehaviour
{
    private List<MiniCube> _faceCube = new List<MiniCube>();
    private BoxCollider _boxCollider;

    private void Awake() {
        _boxCollider = GetComponent<BoxCollider>();
    }

    
    private void OnTriggerEnter(Collider other) {
        int layerCollided = other.gameObject.layer;

        _CheckCollisionMiniCube(other.gameObject, layerCollided);
    }

    private void _CheckCollisionMiniCube(GameObject collidedGameobject, int layerCollided) {
        if (LayerMask.LayerToName(layerCollided) == "MiniCube") {
            var miniCube = collidedGameobject.GetComponent<MiniCube>();
            _faceCube.Add(miniCube);

            // Here's win condition, if the 6 faces returns true.
            if ((GameManager.Instance.numberCube * 2) == _faceCube.Count) {
                GameManager.Instance.CheckWin(_CheckForwardFace());
            }
        }
    }

    private bool _CheckForwardFace()
    {
        Vector3 tempFwdValue = Vector3.zero;

        // Check if they all have the same forward.
        for (int i = 0; i < _faceCube.Count; i++)
        {
            var forwardCube = _faceCube[i].transform.eulerAngles;

            // If the face have the cube all facing the same direction, it seems the face as the same color
            if (tempFwdValue == Vector3.zero)
            {
                tempFwdValue = forwardCube;
            }

            if(tempFwdValue != forwardCube)
            {
                Debug.Log("Face doesn't have the same colors, no victory");
                return false;
            }
        }

        return true;
    }

    public void ActivateColliders() {
        _boxCollider.enabled = true;
    }

    public void ResetColliders() {
        if(_boxCollider != null)
            _boxCollider.enabled = false;

        _faceCube.Clear();
    }

} 
