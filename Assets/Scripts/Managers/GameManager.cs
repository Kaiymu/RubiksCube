using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private GameObject _centerOfCube;

    public GameObject CenterOfCube {
        get { return _centerOfCube; }
    }

    public enum GAME_STATE { SCRAMBLED, PLAYING, WIN, MAIN_MENU }
    public GAME_STATE gameState = GAME_STATE.SCRAMBLED;

    [Header("Number of cube to spawn X*X*X")]
    [Range(2, 6)]
    public int numberCube = 2;

    [Header("Number of random direction and positive that the cubes will do before starting the game")]
    public int numberRandomScramble = 10;

    private void Awake() {
        if (Instance == null)
            Instance = this;
        else {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(this);
    }

    private void OnEnable() {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // TODO not keeping the scene logic here, might create a SceneManager for that, only for test purpose for now
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        if(scene.name == "SampleScene") {
            gameState = GAME_STATE.SCRAMBLED;
            _CreateCenterOfCube();
        }

        if (scene.name == "MenuScene") {
            gameState = GAME_STATE.MAIN_MENU;
        }
    }

    // TODO To remove, only for test purpose
    private void Update() {
        if (Input.GetKeyDown(KeyCode.A)) {
            SceneManager.LoadScene(1);
        }

        if (Input.GetKeyDown(KeyCode.Z)) {
            SceneManager.LoadScene(0);
        }
    }

    private void _CreateCenterOfCube() {
        if(gameState != GAME_STATE.MAIN_MENU) {

            float centerOfCubeValue = (numberCube - 1) / 2f;
            Vector3 centerOfCubePos = new Vector3(centerOfCubeValue, centerOfCubeValue, centerOfCubeValue);
            _centerOfCube = new GameObject();
            _centerOfCube.name = "[CENTER OF CUBE]";
            _centerOfCube.transform.position = centerOfCubePos;
        }
    }

}

