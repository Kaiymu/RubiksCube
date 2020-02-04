using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Face : MonoBehaviour
{
    public List<MiniCube> faceCube = new List<MiniCube>();

    public void UpdateCubes(MiniCube newMiniCubeFace)
    {
        faceCube.Clear();
        faceCube.Add(newMiniCubeFace);

        _CheckColorsFace();
    }

    private void _CheckColorsFace()
    {
        CubeManager.COLORS tempColor = CubeManager.COLORS.NONE;
        bool allSameColor = false;

        for (int i = 0; i < faceCube.Count; i++)
        {
            var colorCube = faceCube[i].GetCurrentFacedColor(transform.forward);
            if (tempColor == CubeManager.COLORS.NONE)
            {
                tempColor = colorCube;
            }

            if(tempColor == colorCube)
            {
                allSameColor = true;
                Debug.Log("Face : " + gameObject.name + " has the same color");
            } else
            {
                Debug.Log("Face doesn't have the same colors");
                allSameColor = false;
                break;
            }
        }
    }

} 
