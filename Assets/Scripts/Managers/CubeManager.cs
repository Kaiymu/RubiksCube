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

    // Parent that gets rotated and get row / column of cubes as children temporarly 
    private Dictionary<string, Transform> _parentTransform = new Dictionary<string, Transform>();

    // Not doing rotation while we already do one
    private bool _rotate;

    // TODO might move all the rotation to an other file for more clarity
    // Undo rotation
    private Stack<UndoRotation> _undoRotation = new Stack<UndoRotation>();
    private bool _undo = true;

    private class UndoRotation
    {
        public Vector3 direction = Vector3.zero;
        public int columnRowDepth = -1;
    }

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
            Rotate(new Vector2(0, ySize), Vector2.up);
        }

        if (Input.GetKeyDown(KeyCode.Z)) {
            Rotate(new Vector2(xSize,0), Vector2.right);
        }

        if (Input.GetKeyDown(KeyCode.R)) {
            RotateDepth(xSize, Vector3.forward);
        }

        // To test basic undo easely on editor mode.
        if (Input.GetKeyDown(KeyCode.E)) {
            _UndoLastRotation();
        }
    }

    public void Rotate(Vector2 rowColumn, Vector3 direction) {
        if (_rotate)
            return;

        var miniCubes = new List<MiniCube>();
        int xyz = 0;
        for (int i = 0; i < _miniCubes.Count; i++) {
            // We round them to int because of floating precision issue.
            int miniCubePos;
            var miniCube = _miniCubes[i];
            if (direction.y != 0) {
                miniCubePos = Mathf.RoundToInt(miniCube.transform.position.y);
                if (miniCubePos == rowColumn.y) {
                    xyz = (int)rowColumn.y;
                    miniCube.xyz = new Vector3Int(miniCube.xyz.x, miniCubePos, miniCube.xyz.z);
                    miniCubes.Add(miniCube);
                }
            }
            else if (direction.x != 0) {
                miniCubePos = Mathf.RoundToInt(miniCube.transform.position.x);
                if (miniCubePos == rowColumn.x) {
                    xyz = (int)rowColumn.x;
                    miniCube.xyz = new Vector3Int(miniCubePos, miniCube.xyz.y, miniCube.xyz.z);
                    miniCubes.Add(miniCube);
                }
            }

        }

        // TODO use the direction and not send a hardcoded vector.
        _RotateParentCube(miniCubes, direction, xyz);
    }


    // TODO huge refactor needed to prevent having 3 time the same functions.
    public void RotateDepth(int z, Vector3 direction) {
        var miniCubeDown = new List<MiniCube>();

        for (int i = 0; i < _miniCubes.Count; i++) {
            var miniCube = _miniCubes[i];
            // We round them to int because of floating precision issue.
            int miniCubeDepth = Mathf.RoundToInt(miniCube.transform.position.z);

            // Getting all the cubes from the same depth
            if (miniCubeDepth == z) {
                Debug.Log("Depth " + miniCubeDepth);
                miniCube.xyz = new Vector3Int(miniCube.xyz.x, miniCube.xyz.y, miniCubeDepth);
                miniCubeDown.Add(_miniCubes[i]);
            }
        }

        // TODO use the direction and not send a hardcoded vector.
        _RotateParentCube(miniCubeDown, direction, z);
    }

    private void _RotateParentCube(List<MiniCube> miniCubeRotation, Vector3 direction, int xyz) {

        Transform parentTransform = null;

        string parentName = "";
        if (Mathf.Abs(direction.x) > 0) {
            parentName = "row" + xyz;
        }
        else if (Mathf.Abs(direction.y) > 0) {
            parentName = "column" + xyz;
        }
        else if (Mathf.Abs(direction.z) > 0) {
            parentName = "depth" + xyz;
        }

        // Getting our parent from our dictionnary
        if (_parentTransform.TryGetValue(parentName, out parentTransform)) {

            // Settings our parents for all the childs that we got
            // TODO : Refactor to get that throught LINQ.
            for (int i = 0; i < miniCubeRotation.Count; i++) {
                miniCubeRotation[i].transform.parent = parentTransform.transform;
            }

            Vector3 rotationValue = Vector3.Scale(direction, new Vector3(90, 90, 90));
            rotationValue += parentTransform.localEulerAngles;

            _rotate = true;

            LeanTween.rotate(parentTransform.gameObject, rotationValue, 0.5f)
            .setOnComplete(() => {
                for (int i = 0; i < miniCubeRotation.Count; i++) {
                    miniCubeRotation[i].transform.parent = null;
                }
                _rotate = false;
            });

            if (_undo)
                _SaveLastMovement(direction, xyz);
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
            Vector3 positionVerticalParent = Vector3.Lerp(new Vector3(0, i, 0), new Vector3(numberCubes - 1, i, numberCubes - 1), 0.5f);
            Vector3 positionHorizontalParent = Vector3.Lerp(new Vector3(i, 0, 0), new Vector3(i, numberCubes - 1, numberCubes - 1), 0.5f);
            Vector3 positionDepthParent = Vector3.Lerp(new Vector3(0, 0, i), new Vector3(numberCubes - 1, numberCubes - 1, i), 0.5f);
            
            // We create our objects, set it's position and add it to the dictionnary while giving it a unique name.
            var parentRotationX = new GameObject();
            var parentRotationY = new GameObject();
            var parentRotationZ = new GameObject();

            parentRotationX.transform.localEulerAngles = Vector3.zero;
            parentRotationX.transform.localPosition = positionVerticalParent;
            parentRotationX.name = "column" + i;

            parentRotationY.transform.localEulerAngles = Vector3.zero;
            parentRotationY.transform.localPosition = positionHorizontalParent;
            parentRotationY.name = "row" + i;

            parentRotationZ.transform.localEulerAngles = Vector3.zero;
            parentRotationZ.transform.localPosition = positionDepthParent;
            parentRotationZ.name = "depth" + i;

            _parentTransform.Add(parentRotationX.name, parentRotationX.transform);
            _parentTransform.Add(parentRotationY.name, parentRotationY.transform);
            _parentTransform.Add(parentRotationZ.name, parentRotationZ.transform);
        }

    }

    ///<summary>
    /// Save the last movement done by the player
    /// Taking direction and row / column as parameter
    ///</summary>
    private void _SaveLastMovement(Vector3 direction, int xyz) {
        UndoRotation undoRotation = new UndoRotation();
        undoRotation.direction = direction;
        undoRotation.columnRowDepth = xyz;
        _undoRotation.Push(undoRotation);
    }

    ///<summary>
    /// Undo the last movement done by the player by inverting the last direction
    ///</summary>
    private void _UndoLastRotation() {
        // Stack is empty, no need to undo
        if (_undoRotation.Count == 0)
            return;

        var undoRotation = _undoRotation.Pop();

        // We invert the direction
        undoRotation.direction *= - 1;
        // TODO maybe a more elegant way of preventing a save while undoing
        _undo = false;

        // Direction tell us if it's horitonzal / vertical movement
        if (Mathf.Abs(undoRotation.direction.x) > 0)
            Rotate(new Vector2(undoRotation.columnRowDepth, 0), undoRotation.direction);
        else if (Mathf.Abs(undoRotation.direction.y) > 0)
            Rotate(new Vector2(undoRotation.columnRowDepth, 0), undoRotation.direction);
        else if (Mathf.Abs(undoRotation.direction.z) > 0)
            RotateDepth((int)undoRotation.columnRowDepth, undoRotation.direction);

        _undo = true;
    }
}
