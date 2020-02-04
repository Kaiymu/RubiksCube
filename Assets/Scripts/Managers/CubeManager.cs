using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CubeManager : MonoBehaviour
{
    [Header("Mini cube prefab")]
    public MiniCube miniCube;

    [Header("Mini cube prefab")]
    public Face facePrefab;

    private int _numberCubes = 2;

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

    // Storing all our mini cubes / faces here
    private List<MiniCube> _miniCubes = new List<MiniCube>();
    private Dictionary<Vector3, Face> _faces = new Dictionary<Vector3, Face>();

    // Parent that gets rotated and get row / column of cubes as children temporarly 
    private Dictionary<string, Transform> _parentTransform = new Dictionary<string, Transform>();

    // Used to make the cube rotate 90 degrees in any axe
    private readonly Vector3 _ROTATION_CUBE = new Vector3(90f, 90f, 90f);
    // Flag to prevent doing rotation while we already do one
    private bool _canRotate = true;

    // Undo rotation
    private Stack<Rotation> _undoRotation = new Stack<Rotation>();
    private bool _saveLastMovement = true;

    // Random direction / row to scramble the cube
    private Stack<Rotation> _randomScrambledDirRowColumnDepth = new Stack<Rotation>();

    public enum COLORS { NONE, GREEN, YELLOW, RED, WHITE, BLUE, ORANGE }

    [Header("LeanTween parameters")]
    public float rotationTimePlaying = 0.5f;
    public float rotationTimeScrambled = 0.0f;
    public LeanTweenType leanTweenEase = LeanTweenType.linear;

    private class Rotation
    {
        public Vector3 direction = Vector3.zero;
        public Vector3 columnRowDepth = Vector3.zero;
    }

    public static CubeManager Instance;

    #region INIT
    private void Awake() {
        if (Instance == null)
            Instance = this;

        _CreateMiniCubes();
        _CreateParentRotation();
    }

    ///<summary>
    /// Create our cubes and add them inside our list.
    /// Each cubes will have a set of basic position, we assume they are 1x1x1 on scale.
    ///</summary>
    private void _CreateMiniCubes() {
        // Because of floating point imprecision i'd rather use int that floats
        Vector3Int cubePosition = Vector3Int.zero;
        _numberCubes = GameManager.Instance.numberCube;

        _miniCubeParent = new GameObject();
        _miniCubeParent.name = "[MINI CUBE PARENT]";

        for (int i = 0; i < _numberCubes; i++) {
            for (int j = 0; j < _numberCubes; j++) {
                for (int k = 0; k < _numberCubes; k++) {
                    
                    // Not creating mini cubes that are "inside" the cube and that the player cannot see
                    if((k > 0 && k < _numberCubes - 1) &&
                        (j > 0 && j < _numberCubes - 1) &&
                            (i > 0 && i < _numberCubes - 1))
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
    /// Create a parent for each row / columns at the center of each.
    /// They're used as temporary pivot point for our MiniCubes
    /// We can access these parents throught a Dictionnary
    ///</summary>
    private void _CreateParentRotation() {

        var parentColumnRowDepth = new GameObject();
        parentColumnRowDepth.name = "[PARENT COLUMN ROW DEPTH]";
        // TODO : Refactor to only do one loop.
        for (int i = 0; i < _numberCubes; i++) {
            // We get the middle position of our farthest cubes in the current vertical row.
            Vector3 positionVerticalParent = Vector3.Lerp(new Vector3(0, i, 0), new Vector3(_numberCubes - 1, i, _numberCubes - 1), 0.5f);
            Vector3 positionHorizontalParent = Vector3.Lerp(new Vector3(i, 0, 0), new Vector3(i, _numberCubes - 1, _numberCubes - 1), 0.5f);
            Vector3 positionDepthParent = Vector3.Lerp(new Vector3(0, 0, i), new Vector3(_numberCubes - 1, _numberCubes - 1, i), 0.5f);

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

    private void Start() {
        _CreateScrambledRubikCubes();
    }

    private void _CreateScrambledRubikCubes() {
        if (GameManager.Instance.gameState == GameManager.GAME_STATE.SCRAMBLED) {
            Vector3 randomDirection;
            Vector3 randomRowColumnDepth;
            Rotation randomRotation;

            for (int i = 0; i < GameManager.Instance.numberRandomScramble; i++) {
                float randomRangeDir = Random.Range(0, 3);
                randomDirection = Vector3.zero;

                int randomSignRange = Random.Range(0, 1);

                if(randomSignRange == 0) {
                    randomSignRange = 1;
                } else {
                    randomSignRange = -1;
                }

                if (randomRangeDir == 0) {
                    randomDirection.x = 1 * randomSignRange;
                } else if (randomRangeDir == 1) {
                    randomDirection.y = 1 * randomSignRange;
                }
                else if (randomRangeDir == 2) {
                    randomDirection.z = 1 * randomSignRange;
                }

                randomRowColumnDepth = new Vector3(Random.Range(0, _numberCubes), Random.Range(0, _numberCubes), Random.Range(0, _numberCubes));

                randomRotation = new Rotation();
                randomRotation.direction = randomDirection;
                randomRotation.columnRowDepth = randomRowColumnDepth;
                _randomScrambledDirRowColumnDepth.Push(randomRotation);
            }

            _UnstackRandomDirection();
            // We scrambled the rubik cube, we can start the game :)

            randomRotation = null;
        }
    }

    private void _UnstackRandomDirection() {
        // Stack is empty, no need to undo
        if (_randomScrambledDirRowColumnDepth.Count == 0) {
            Debug.Log("Scrambled stack empty, game can start");
            GameManager.Instance.gameState = GameManager.GAME_STATE.PLAYING;
            return;
        }

        var undoRotation = _randomScrambledDirRowColumnDepth.Pop();
        _Rotate(undoRotation.columnRowDepth, undoRotation.direction);
    }

    #endregion

    #region FACE
    ///<summary>
    /// Creating all the faces, with rotating and unique vector forward for each
    ///</summary>
    private void _CreatingFaces() {
        Dictionary<Vector3, Vector3> _faceList = new Dictionary<Vector3, Vector3>();

        // TODO a lot of magic numbers, needs to be handled better.
        Vector3 frontFace = Vector3.Lerp(new Vector3(0, 0, 0), new Vector3(_numberCubes - 1, _numberCubes - 1, -0.5f), 0.5f); ; // Front face
        Vector3 backwardFace = Vector3.Lerp(new Vector3(0, 0, 0), new Vector3(_numberCubes - 1, _numberCubes - 1, (_numberCubes * 2) - 1.5f), 0.5f); // Backward face
        Vector3 upFace = Vector3.Lerp(new Vector3(0, 0, 0), new Vector3(_numberCubes - 1, (_numberCubes * 2) - 1.5f, _numberCubes - 1f), 0.5f); // Top Face
        Vector3 downFace = Vector3.Lerp(new Vector3(0, 0, 0), new Vector3(_numberCubes - 1, -0.5f, _numberCubes - 1), 0.5f); // Down face
        Vector3 rightFace = Vector3.Lerp(new Vector3(0, 0, 0), new Vector3(-0.5f, _numberCubes - 1, _numberCubes - 1), 0.5f); // Right face
        Vector3 leftFace = Vector3.Lerp(new Vector3(0, 0, 0), new Vector3((_numberCubes* 2) -1.5f, _numberCubes - 1, _numberCubes - 1), 0.5f); // Left face

        _faceList.Add(Vector3.forward, frontFace);
        _faceList.Add(Vector3.back, backwardFace);
        _faceList.Add(Vector3.up, upFace);
        _faceList.Add(Vector3.down, downFace);
        _faceList.Add(Vector3.right, rightFace);
        _faceList.Add(Vector3.left, leftFace);

        var parentGameobjectFace = new GameObject();
        parentGameobjectFace.name = "[FACE PARENT]";

        foreach(var faceElement in _faceList) {
            var face = Instantiate(facePrefab, Vector3.zero, Quaternion.identity);
            face.ResetColliders();
            face.transform.parent = parentGameobjectFace.transform;
            face.transform.position = faceElement.Value;
            face.transform.localScale = new Vector3(_numberCubes, _numberCubes, 0.5f);
            face.transform.forward = faceElement.Key;
            face.name = "Face : " + faceElement.Key;
            _faces.Add(faceElement.Key, face);
        }

        // Removing the temporary dictionnary
        _faceList.Clear();
        _faceList = null;
    }

    #endregion

    #region ROTATION
    ///<summary>
    /// Sending a direction to make a rotation between xyz
    ///</summary>
    public void Rotate(Vector3 direction)
    {
        if (!_canRotate || _miniCubeSelected == null)
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
        if (!_canRotate)
            return;

        // We create a list of mini cube that is going to be filled with the filtered ones.
        var miniCubes = new List<MiniCube>();

        for (int i = 0; i < _miniCubes.Count; i++) {
            // We round them to int because of floating precision issue.
            int miniCubePos;
            var miniCube = _miniCubes[i];

            if (direction.x != 0)
            {
                miniCubePos = Mathf.RoundToInt(miniCube.transform.position.x);
                if (miniCubePos == rowColumnDepth.x)
                {
                    miniCube.xyz = new Vector3Int(miniCubePos, miniCube.xyz.y, miniCube.xyz.z);
                    miniCubes.Add(miniCube);
                }
            }
            else if (direction.y != 0)
            {
                miniCubePos = Mathf.RoundToInt(miniCube.transform.position.y);
                if (miniCubePos == rowColumnDepth.y)
                {
                    miniCube.xyz = new Vector3Int(miniCube.xyz.x, miniCubePos, miniCube.xyz.z);
                    miniCubes.Add(miniCube);
                }
            }
            else if (direction.z != 0)
            {
                miniCubePos = Mathf.RoundToInt(miniCube.transform.position.z);
                if (miniCubePos == rowColumnDepth.z)
                {
                    miniCube.xyz = new Vector3Int(miniCube.xyz.x, miniCube.xyz.y, miniCubePos);
                    miniCubes.Add(miniCube);
                }
            }
        }

        _RotateParentCube(miniCubes, direction, rowColumnDepth);
    }

    private void _RotateParentCube(List<MiniCube> miniCubeRotation, Vector3 direction, Vector3 xyz) {

        Transform parentTransform = null;

        string parentName = "";
        if (Mathf.Abs(direction.x) > 0) {
            parentName = "row" + xyz.x;
        }
        else if (Mathf.Abs(direction.y) > 0) {
            parentName = "column" + xyz.y;
        }
        else if (Mathf.Abs(direction.z) > 0) {
            parentName = "depth" + xyz.z;
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

            _canRotate = false;
            foreach(var face in _faces) {
                face.Value.ResetColliders();
            }

            float rotationTime = 0f;
            if (GameManager.Instance.gameState == GameManager.GAME_STATE.SCRAMBLED)
                rotationTime = rotationTimeScrambled;
            else if (GameManager.Instance.gameState == GameManager.GAME_STATE.PLAYING) {
                rotationTime = rotationTimePlaying;
            }

            LeanTween.rotateAround(parentTransform.gameObject, direction, _ROTATION_CUBE.x, rotationTime)
            .setEase(leanTweenEase)
            .setOnComplete(() => {
                // We unparent the cubes and unblock the rotation
                _canRotate = true;
                for (int i = 0; i < miniCubeRotation.Count; i++) {
                    miniCubeRotation[i].transform.parent = _miniCubeParent.transform;
                }

                if (GameManager.Instance.gameState == GameManager.GAME_STATE.SCRAMBLED) {
                    _UnstackRandomDirection();
                }

                if (GameManager.Instance.gameState == GameManager.GAME_STATE.PLAYING) {
                    foreach (var face in _faces) {
                        face.Value.ActivateColliders();
                    }
                }
            });

            // If we want to save the last movement
            if (_saveLastMovement && GameManager.Instance.gameState == GameManager.GAME_STATE.PLAYING)
                _SaveLastMovement(direction, xyz);
        }
    }

    #endregion
    #region UNDO
    ///<summary>
    /// Save the last movement done by the player
    /// Taking direction and row / column as parameter
    ///</summary>
    private void _SaveLastMovement(Vector3 direction, Vector3 xyz) {
        Rotation undoRotation = new Rotation();
        undoRotation.direction = direction;
        undoRotation.columnRowDepth = xyz;
        _undoRotation.Push(undoRotation);
    }

    ///<summary>
    /// Undo the last movement done by the player by inverting the last direction
    ///</summary>
    public void UndoLastRotation() {
        if (_canRotate)
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
        _Rotate(undoRotation.columnRowDepth, undoRotation.direction);

        _saveLastMovement = true;
    }
    #endregion
}
