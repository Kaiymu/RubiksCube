using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CubeManager : MonoBehaviour
{
    public MiniCube miniCube;
    public int numberCubes = 2;

    // TODO : Remove, just for test purpose before getting the column / row of the cube
    public int ySize;
    public int xSize;

    private List<MiniCube> _miniCubes = new List<MiniCube>();

    private Dictionary<string, Transform> _parentTransform = new Dictionary<string, Transform>();

    public static CubeManager Instance;

    private void Awake() {
        if (Instance == null)
            Instance = this;

        _CreateCubes();
        _CreateParentRotation();
    }

    ///<summary>
    /// Create our cubes and add them inside our list.
    /// Each cubes will have a set of basic position, we assume they are 1x1x1 on scale.
    ///</summary>
    private void _CreateCubes() {
        // Because of floating point imprecision i'd rather use int that floats
        Vector3Int cubePosition = Vector3Int.zero;

        for (int i = 0; i < numberCubes; i++) {
            for (int j = 0; j < numberCubes; j++) {
                for (int k = 0; k < numberCubes; k++) {
                    // X, Y, Z
                    cubePosition = new Vector3Int(k, j, i);
                    MiniCube newMiniCube = Instantiate(miniCube, cubePosition, Quaternion.identity);
                    newMiniCube.xyz = cubePosition;
                    newMiniCube.transform.position = cubePosition;
                    _miniCubes.Add(newMiniCube);
                }
            }
        }
    }
    private void Update() {
        // Just to test basic rotation, the drag code to touch our cubes will replace that
        if (Input.GetKeyDown(KeyCode.A)) {
            RotateHorizontal(ySize, Vector2.up);
        }

        if (Input.GetKeyDown(KeyCode.Z)) {
            RotateVertical(xSize, Vector2.right);
        }
    }

    // TODO : Refactor to get one function for horizontal vertical
    public void RotateHorizontal(int y, Vector2 direction) {
        var miniCubeHorizontal = new List<MiniCube>();

        for (int i = 0; i < _miniCubes.Count; i++) {
            // We round them to int because of floating precision issue.
            int miniCubeYPos = Mathf.RoundToInt(_miniCubes[i].transform.position.y);
            if (miniCubeYPos == y) {
                Debug.Log("Row " + miniCubeYPos);
                miniCube.xyz = new Vector3Int(miniCube.xyz.x, miniCubeYPos, miniCube.xyz.z);
                miniCubeHorizontal.Add(_miniCubes[i]);

            }
        }

        _RotateParentCube(miniCubeHorizontal, false, direction);
    }


    // TODO : Refactor to get one function for horizontal vertical
    public void RotateVertical(int x, Vector2 direction) {
        var miniCubeVertical = new List<MiniCube>();

        for (int i = 0; i < _miniCubes.Count; i++) {
            var miniCube = _miniCubes[i];
            // We round them to int because of floating precision issue.
            int miniCubeXPos = Mathf.RoundToInt(miniCube.transform.position.x);
            
            // Getting all the cubes from the same row
            if (miniCubeXPos == x) {
                Debug.Log("Column " + miniCubeXPos);
                miniCube.xyz = new Vector3Int(miniCubeXPos, miniCube.xyz.y, miniCube.xyz.z);
                miniCubeVertical.Add(_miniCubes[i]);
            }
        }

        _RotateParentCube(miniCubeVertical, true, direction);
    }


    private void _RotateParentCube(List<MiniCube> miniCubeRotation, bool vertical, Vector2 direction) {

        Transform parentTransform = null;

        string parentName = vertical ? "row" + ySize : "column" + xSize;

        // Getting our parent from our dictionnary
        if (_parentTransform.TryGetValue(parentName, out parentTransform)) {

            // Settings our parents for all the childs that we got
            // TODO : Refactor to get that throught LINQ.
            for (int i = 0; i < miniCubeRotation.Count; i++) {
                miniCubeRotation[i].transform.parent = parentTransform.transform;
            }

            // We only go one way, but when we'll have our drag direction
            if (vertical) {
                var rotatingVector = direction * new Vector3(90, 0, 0);
                parentTransform.transform.Rotate(rotatingVector);
            }
            else {
                var rotatingVector = direction * new Vector3(0, 90, 0);
                parentTransform.transform.Rotate(rotatingVector);
            }

            // Unsetting all the parents
            for (int i = 0; i < miniCubeRotation.Count; i++) {
                miniCubeRotation[i].transform.parent = null;
            }
        }
        
    }

    ///<summary>
    /// Create a parent for each row / columns at the center of each.
    /// They're used as temporary pivot point for our MiniCubes
    /// We can access these parents throught a Dictionnary
    ///</summary>
    private void _CreateParentRotation() {

        // TODO : Refactor to only do one loop.
        for (int i = 0; i < numberCubes; i++) {
            // We get the middle position of our farthest cubes in the current vertical row.
            Vector3 positionHVerticalParent = Vector3.Lerp(new Vector3(0, i, 0), new Vector3(numberCubes - 1, i, numberCubes - 1), 0.5f);

            // We create our object, set it's position and add it to the dictionnary while giving it a unique name.
            var parentRotation = new GameObject();
            parentRotation.transform.localEulerAngles = Vector3.zero;
            parentRotation.transform.localPosition = positionHVerticalParent;
            parentRotation.name = "column" + i;
            _parentTransform.Add(parentRotation.name, parentRotation.transform);
        }

        for (int i = 0; i < numberCubes; i++) {
            Vector3 positionHorizontalParent = Vector3.Lerp(new Vector3(i, 0, 0), new Vector3(i, numberCubes - 1, numberCubes - 1), 0.5f);

            var fakeParent = new GameObject();
            fakeParent.transform.localEulerAngles = Vector3.zero;
            fakeParent.transform.localPosition = positionHorizontalParent;
            fakeParent.name = "row" + i;
            _parentTransform.Add(fakeParent.name, fakeParent.transform);
        }
    }
}
