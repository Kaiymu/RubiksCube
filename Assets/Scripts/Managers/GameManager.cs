using System;
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

    private GAME_STATE _gameState = GAME_STATE.MAIN_MENU;
    public GAME_STATE gameState
    {
        get { return _gameState; }
        set { Debug.Log("Game manager goes from state : " + _gameState + " to : " + value);
            _gameState = value; }
    }

    [Header("Number of cube to spawn X*X*X")]
    [Range(2, 6)]
    public int numberCube = 2;

    [Header("Number of random direction and positive that the cubes will do before starting the game")]
    public int numberRandomScramble = 10;

    private float _seconds = 0;

    // TimeSpan to handle the tumer
    public TimeSpan TimerSecond
    {
        get
        {
            return TimeSpan.FromSeconds(_seconds);
        }
    }

    private void Awake() {
        if (Instance == null)
            Instance = this;
        else {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(this);

        _CreateCenterOfCube();
    }

    private void Start()
    {

    }

    // We register / unregister to OnSceneLoad event to load specific events
    private void OnEnable() {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Update()
    {
        // Only updating the timer when the game is going on 
        if (_gameState == GAME_STATE.PLAYING)
        {
            _seconds += Time.deltaTime;
            if(DatasManager.Instance != null)
                DatasManager.Instance.cubeSaveContainer.timerInSeconds = _seconds;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        if (scene.buildIndex == 0) { 
            gameState = GAME_STATE.MAIN_MENU;
        }

        if (scene.buildIndex == 1)
        {
            // If no data is found, we load the game as "Scrambled", which means that it's going to start
            if(DatasManager.Instance == null)
            {
                gameState = GAME_STATE.SCRAMBLED;
                return;
            }

            // If data exist, but it's empty, we just start the new game
            if(DatasManager.Instance.cubeSaveContainer.miniCubeSaveList.Count == 0)
                gameState = GAME_STATE.SCRAMBLED;
            else if (DatasManager.Instance.cubeSaveContainer.miniCubeSaveList.Count > 0) {
                _seconds = DatasManager.Instance.cubeSaveContainer.timerInSeconds;
                gameState = GAME_STATE.PLAYING;
            }

            _CreateCenterOfCube();
        }
    }

    ///<summary>
    /// When in game, calculate the center of the cube for the camera to look at
    ///</summary>
    private void _CreateCenterOfCube() {
        float centerOfCubeValue = (numberCube - 1) / 2f;
        Vector3 centerOfCubePos = new Vector3(centerOfCubeValue, centerOfCubeValue, centerOfCubeValue);
        _centerOfCube = new GameObject();
        _centerOfCube.name = "[CENTER OF CUBE]";
        _centerOfCube.transform.position = centerOfCubePos;
    }

}

