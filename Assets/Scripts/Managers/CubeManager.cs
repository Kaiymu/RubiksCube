using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CubeManager : MonoBehaviour
{
    [Header("Mini cube prefab")]
    public MiniCube miniCube;
    [Header("Number of cube to spawn at first")]
    // TODO Make it changeable via the main menu at boot
    public int numberCubes = 2;

    private MiniCube _miniCubeSelected;
    private GameObject _miniCubeParent;

    public MiniCube MiniCubeSelected
    {
        get { return _miniCubeSelected; }
        set {
            // Unsellecting the previous cube if it exist
            if (_miniCubeSelected != null)
                _miniCubeSelected.selected = false;

            if (value) {
                _miniCubeSelected = value;
                _miniCubeSelected.selected = true;
                Debug.Log("MiniCube selected : " + value);
            }
            else {
                Debug.LogError("Trying to set a null cube");
            };
        }
    }

    // Storing all our cubes here
    private List<MiniCube> _miniCubes = new List<MiniCube>();
    private Dictionary<Vector3, Face> _faces = new Dictionary<Vector3, Face>();

    // Parent that gets rotated and get row / column of cubes as children temporarly 
    private Dictionary<string, Transform> _parentTransform = new Dictionary<string, Transform>();

    // Used to make the cube rotate 90 degrees in any axe
    private readonly Vector3 _ROTATION_CUBE = new Vector3(90f, 90f, 90f);
    // Not doing rotation while we already do one
    private bool _rotate;

    // TODO might move all the rotation to an other file for more clarity
    // Undo rotation
    private Stack<UndoRotation> _undoRotation = new Stack<UndoRotation>();
    private bool _saveLastMovement = true;

    // TODO might put it global and constant
    private List<Vector3> _faceList = new List<Vector3>();
    public enum COLORS { NONE, GREEN, YELLOW, RED, WHITE, BLUE, ORANGE }

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

        _miniCubeParent = new GameObject();
        _miniCubeParent.name = "[MINI CUBE PARENT]";

        for (int i = 0; i < numberCubes; i++) {
            for (int j = 0; j < numberCubes; j++) {
                for (int k = 0; k < numberCubes; k++) {
                    
                    // Not creating mini cubes that are "inside" the cube and that the player cannot see
                    if((k > 0 && k < numberCubes - 1) &&
                        (j > 0 && j < numberCubes - 1) &&
                            (i > 0 && i < numberCubes - 1))
                    {
                        continue;
                    }


                    cubePosition = new Vector3Int(k, j, i);
                    MiniCube newMiniCube = Instantiate(miniCube, cubePosition, Quaternion.identity);
                    newMiniCube.xyz = cubePosition;
                    newMiniCube.transform.position = cubePosition;
                    newMiniCube.transform.parent = _miniCubeParent.transform;
                    _miniCubes.Add(newMiniCube);

                }
            }
        }

        _CreatingFaces();
    }

    ///<summary>
    /// Sending a direction to make a rotation between xyz
    ///</summary>
    private void _CreatingFaces() {

        // TODO might put that globally to be also used by the MiniCube elements
        _faceList.Add(Vector3.forward);
        _faceList.Add(Vector3.back);
        _faceList.Add(Vector3.up);
        _faceList.Add(Vector3.down);
        _faceList.Add(Vector3.right);
        _faceList.Add(Vector3.left);

        var parentGameobjectFace = new GameObject();
        parentGameobjectFace.name = "[FACE PARENT]";

        for (int i = 0; i < _faceList.Count; i++)
        {
            var faceObject = new GameObject();
            var face = faceObject.AddComponent<Face>();
            face.transform.parent = parentGameobjectFace.transform;
            face.transform.forward = _faceList[i];
            face.name = "Face : " + _faceList[i];
            _faces.Add(_faceList[i], face);
        }

        
        for(int i = 0; i < _miniCubes.Count; i++)
        {
            var miniCube = _miniCubes[i];

            // TODO Find a better way to select all the faces, maybe with a colliders spawned.
            // First face
            /*
            if ((miniCube.transform.position.x >= 0 &&
                miniCube.transform.position.x <= numberCubes - 1)
                && (miniCube.transform.position.y >= 0 &&
                miniCube.transform.position.y <= numberCubes - 1)
                && (miniCube.transform.position.z == 0)) {
            }
            */
        }
    }

    ///<summary>
    /// Sending a direction to make a rotation between xyz
    ///</summary>
    public void Rotate(Vector3 direction)
    {
        if (_rotate || _miniCubeSelected == null)
            return;

        // Take the X, Y, Z of the cube to know which row, column and depth to rotate
        Vector3 rowColumnDepth = Vector3.zero;
        rowColumnDepth.x = Mathf.RoundToInt(_miniCubeSelected.transform.position.x);
        rowColumnDepth.y = Mathf.RoundToInt(_miniCubeSelected.transform.position.y);
        rowColumnDepth.z = Mathf.RoundToInt(_miniCubeSelected.transform.position.z);

        _Rotate(rowColumnDepth, direction);
    }

    ///<summary>
    /// Make the rotation happen by taking a direction and a column / row / depth to turn
    /// <param name=rowColumnDepth>Vector3 taking a value to know which column, row or depth should be turned</param>
    /// <param name=direction>Ranging from -1 to 1, to know which way to turn our element</param>
    ///</summary>
    private void _Rotate(Vector3 rowColumnDepth, Vector3 direction) {
        // If already rotating, we do nothing
        if (_rotate || _miniCubeSelected == null)
            return;

        // We create a list of mini cube that is going to be filled with the filtered ones.
        var miniCubes = new List<MiniCube>();

        // To know which 
        int xyz = 0;

        for (int i = 0; i < _miniCubes.Count; i++) {
            // We round them to int because of floating precision issue.
            int miniCubePos;
            var miniCube = _miniCubes[i];

            if (direction.x != 0)
            {
                miniCubePos = Mathf.RoundToInt(miniCube.transform.position.x);
                if (miniCubePos == rowColumnDepth.x)
                {
                    xyz = (int)rowColumnDepth.x;
                    miniCube.xyz = new Vector3Int(miniCubePos, miniCube.xyz.y, miniCube.xyz.z);
                    miniCubes.Add(miniCube);
                }
            }
            else if (direction.y != 0)
            {
                miniCubePos = Mathf.RoundToInt(miniCube.transform.position.y);
                if (miniCubePos == rowColumnDepth.y)
                {
                    xyz = (int)rowColumnDepth.y;
                    miniCube.xyz = new Vector3Int(miniCube.xyz.x, miniCubePos, miniCube.xyz.z);
                    miniCubes.Add(miniCube);
                }
            }
            else if (direction.z != 0)
            {
                miniCubePos = Mathf.RoundToInt(miniCube.transform.position.z);
                if (miniCubePos == rowColumnDepth.z)
                {
                    xyz = (int)rowColumnDepth.z;
                    miniCube.xyz = new Vector3Int(miniCube.xyz.x, miniCube.xyz.y, miniCubePos);
                    miniCubes.Add(miniCube);
                }
            }
        }

        _RotateParentCube(miniCubes, direction, xyz);
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
            for (int i = 0; i < miniCubeRotation.Count; i++) {
                miniCubeRotation[i].transform.parent = parentTransform.transform;
            }

            // Only one direction is set at the time, so 90 * 1 or -1 gives the value, the rest becomes 0
            Vector3 rotationValue = Vector3.Scale(direction, _ROTATION_CUBE);
            float rotationAngle = rotationValue.x + rotationValue.y + rotationValue.z;

            _rotate = true;

            LeanTween.rotateAround(parentTransform.gameObject, direction, _ROTATION_CUBE.x, 0.5f)
            .setOnComplete(() => {
                // We unparent the cubes and unblock the rotation
                for (int i = 0; i < miniCubeRotation.Count; i++) {
                    miniCubeRotation[i].transform.parent = _miniCubeParent.transform;
                }
                _rotate = false;
            });
            
            // If we want to save the last movement
            if (_saveLastMovement)
                _SaveLastMovement(direction, xyz);
        }
    }

    ///<summary>
    /// Create a parent for each row / columns at the center of each.
    /// They're used as temporary pivot point for our MiniCubes
    /// We can access these parents throught a Dictionnary
    ///</summary>
    private void _CreateParentRotation() {

        var parentColumnRowDepth = new GameObject();
        parentColumnRowDepth.name = "[PARENT COLUMN ROW DEPTH]";
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

            parentRotationX.transform.parent = parentColumnRowDepth.transform;
            parentRotationY.transform.parent = parentColumnRowDepth.transform;
            parentRotationZ.transform.parent = parentColumnRowDepth.transform;

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
    public void UndoLastRotation() {
        if (_rotate)
            return;

        // Stack is empty, no need to undo
        if (_undoRotation.Count == 0)
        {
            Debug.Log("Stack empty no more undo to do");
            return;
        }

        var undoRotation = _undoRotation.Pop();

        // We invert the direction
        undoRotation.direction *= - 1;

        // TODO maybe a more elegant way of preventing a save while undoing
        _saveLastMovement = false;

        // Direction tell us if it's horitonzal / vertical movement
        if (Mathf.Abs(undoRotation.direction.x) > 0)
            _Rotate(new Vector3(undoRotation.columnRowDepth, 0, 0), undoRotation.direction);
        else if (Mathf.Abs(undoRotation.direction.y) > 0)
            _Rotate(new Vector3(0, undoRotation.columnRowDepth, 0), undoRotation.direction);
        else if (Mathf.Abs(undoRotation.direction.z) > 0)
            _Rotate(new Vector3(0, 0, undoRotation.columnRowDepth), undoRotation.direction);

        _saveLastMovement = true;
    }
}
