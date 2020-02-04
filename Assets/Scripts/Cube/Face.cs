using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Face : MonoBehaviour
{
    private List<MiniCube> _faceCube = new List<MiniCube>();

    private void OnTriggerEnter(Collider other) {
        int layerCollided = other.gameObject.layer;

        if(LayerMask.LayerToName(layerCollided) == "MiniCube") {
            var miniCube = other.gameObject.GetComponent<MiniCube>();
            _faceCube.Add(miniCube);
            
            if((CubeManager.Instance.numberCubes * 2) == _faceCube.Count) {
                Debug.LogError(_CheckColorsFace());
            }
        }
    }

    private bool _CheckColorsFace()
    {
        CubeManager.COLORS tempColor = CubeManager.COLORS.NONE;

        for (int i = 0; i < _faceCube.Count; i++)
        {
            var colorCube = _faceCube[i].GetCurrentFacedColor(transform.forward);
            if (tempColor == CubeManager.COLORS.NONE)
            {
                tempColor = colorCube;
            }

            if(tempColor == colorCube)
            {
                Debug.Log("Face : " + gameObject.name + " has the same color");
            } else
            {
                Debug.Log("Face doesn't have the same colors");
                return false;
            }
        }

        return true;
    }

} 
