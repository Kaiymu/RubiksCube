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

        if(LayerMask.LayerToName(layerCollided) == "MiniCube") {
            var miniCube = other.gameObject.GetComponent<MiniCube>();
            _faceCube.Add(miniCube);
            
            if((GameManager.Instance.numberCube * 2) == _faceCube.Count) {
                Debug.LogError(_CheckColorsFace());
            }
        }
    }

    private bool _CheckColorsFace()
    {
        Vector3 tempColor = Vector3.zero;

        // Check if they all have the same forward.
        for (int i = 0; i < _faceCube.Count; i++)
        {
            var forwardCube = _faceCube[i].transform.forward;
            if (tempColor == Vector3.zero)
            {
                tempColor = forwardCube;
            }

            if(tempColor != forwardCube)
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
        _boxCollider.enabled = false;
        _faceCube.Clear();
    }

} 
