using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniCube : MonoBehaviour
{
    public Vector3Int xyz;
    public bool selected;

    [Header("Set all the face according to their color")]
    // Would be better to set it elsewhere because it'll be the same for all cubes, CubeManager could be a better fit
    public List<FaceColor> faceColor = new List<FaceColor>(6);

    [Serializable]
    public class FaceColor
    {
        public Vector3 facingDirection;
        public CubeManager.COLORS colorFace;
    }
}
