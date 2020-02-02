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

    // TODO might move all the rotation to an other file for more clarity
    // Undo rotation
    private Stack<UndoRotation> _undoRotation = new Stack<UndoRotation>();
    private bool _undo = true;

    private class UndoRotation
    {
        public Vector2 direction = Vector3.zero;
        public Vector2 columnRow = new Vector2(-1, -1);
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
            RotateHorizontal(ySize, Vector2.up);
        }

        if (Input.GetKeyDown(KeyCode.Z)) {
            RotateVertical(xSize, Vector2.right);
        }

        if (Input.GetKeyDown(KeyCode.R)) {
            RotateZPos(xSize, new Vector3(0, 0, 1));
        }

        // To test basic undo easely on editor mode.
        if (Input.GetKeyDown(KeyCode.E)) {
            _UndoLastRotation();
        }
    }

    // TODO : Refactor to get one function for horizontal vertical
    public void RotateHorizontal(int y, Vector3 direction) {
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

        // TODO use the direction and not send a hardcoded vector.
        _RotateParentCube(miniCubeHorizontal, direction, new Vector3(0, y, 0));
    }


    // TODO : Refactor to get one function for horizontal vertical
    public void RotateVertical(int x, Vector3 direction) {
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

        // TODO use the direction and not send a hardcoded vector.
        _RotateParentCube(miniCubeVertical, direction, new Vector3(x, 0, 0));
    }

    // TODO huge refactor needed to prevent having 3 time the same functions.
    public void RotateZPos(int z, Vector3 direction) {
        var miniCubeDown = new List<MiniCube>();

        for (int i = 0; i < _miniCubes.Count; i++) {
            var miniCube = _miniCubes[i];
            // We round them to int because of floating precision issue.
            int miniCubeZPos = Mathf.RoundToInt(miniCube.transform.position.z);

            // Getting all the cubes from the same row
            if (miniCubeZPos == z) {
                Debug.Log("Z pos " + miniCubeZPos);
                miniCube.xyz = new Vector3Int(miniCube.xyz.x, miniCube.xyz.y, miniCubeZPos);
                miniCubeDown.Add(_miniCubes[i]);
            }
        }

        // TODO use the direction and not send a hardcoded vector.
        _RotateParentCube(miniCubeDown, direction, new Vector3(0, 0, z));
    }


    private void _RotateParentCube(List<MiniCube> miniCubeRotation, Vector3 direction, Vector3 yx) {

        Transform parentTransform = null;

        string parentName = "";
        if (Mathf.Abs(direction.x) > 0) {
             parentName = "row" + yx.y;
        }
        else if (Mathf.Abs(direction.y) > 0) {
             parentName = "column" + yx.x;
        }
        else if (Mathf.Abs(direction.z) > 0) {
             parentName = "zPos" + yx.z;
        }

        // Getting our parent from our dictionnary
        if (_parentTransform.TryGetValue(parentName, out parentTransform)) {

            // Settings our parents for all the childs that we got
            // TODO : Refactor to get that throught LINQ.
            for (int i = 0; i < miniCubeRotation.Count; i++) {
                miniCubeRotation[i].transform.parent = parentTransform.transform;
            }

            // We only go one way, but when we'll have our drag direction
            // Because direction is either in X or Y, 90 * 0 gives 0 so we only rotate on one side anyway.
            Vector3 rotationValue;
            rotationValue = Vector3.Scale(direction, new Vector3(90, 90, 90));
            Debug.LogError(direction + " " + rotationValue);
            parentTransform.transform.Rotate(rotationValue);

            if(_undo)
                _SaveLastMovement(direction, yx);

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

            var parentRotation = new GameObject();
            parentRotation.transform.localEulerAngles = Vector3.zero;
            parentRotation.transform.localPosition = positionHorizontalParent;
            parentRotation.name = "row" + i;
            _parentTransform.Add(parentRotation.name, parentRotation.transform);
        }

        for (int i = 0; i < numberCubes; i++) {
            Vector3 positionHorizontalParent = Vector3.Lerp(new Vector3(0, 0, i), new Vector3(numberCubes - 1, numberCubes - 1, i), 0.5f);

            var parentRotation = new GameObject();
            parentRotation.transform.localEulerAngles = Vector3.zero;
            parentRotation.transform.localPosition = positionHorizontalParent;
            parentRotation.name = "zPos" + i;
            _parentTransform.Add(parentRotation.name, parentRotation.transform);
        }
    }

    ///<summary>
    /// Save the last movement done by the player
    /// Taking direction and row / column as parameter
    ///</summary>
    private void _SaveLastMovement(Vector2 direction, Vector2 xy) {
        UndoRotation undoRotation = new UndoRotation();
        undoRotation.direction = direction;
        undoRotation.columnRow = xy;
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
            RotateVertical((int)undoRotation.columnRow.x, undoRotation.direction);
        else if (Mathf.Abs(undoRotation.direction.y) > 0)
            RotateHorizontal((int)undoRotation.columnRow.y, undoRotation.direction);

        _undo = true;
    }
}
