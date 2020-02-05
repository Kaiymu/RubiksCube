using UnityEngine;
using System;
using System.Collections.Generic;

public partial class DatasManager : MonoBehaviour
{
    // -----------------------------------------------------------------------------------------
    //  Song Save
    // -----------------------------------------------------------------------------------------
    [Serializable]
    public class MiniCubeSave
    {
        public float timerInSeconds;
        public List<MiniCubeDataSave> miniCubeSaveList = new List<MiniCubeDataSave>();
    }

    [Serializable]
    public class MiniCubeDataSave
    {
        public Vector3Int cubePosition;
        public Vector3Int cubeRotation;
    }
}
