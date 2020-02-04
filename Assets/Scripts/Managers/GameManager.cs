using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private GameObject _centerOfCube;

    public Transform CenterOfCube {
        get { return _centerOfCube.transform; }
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
        _CreateCenterOfCube();
    }

    private void _CreateCenterOfCube() {
        if(gameState != GAME_STATE.MAIN_MENU) {

            float centerOfCubeValue = numberCube / 2;
            Vector3 centerOfCubePos = new Vector3(centerOfCubeValue, centerOfCubeValue, centerOfCubeValue);
            _centerOfCube = new GameObject();
            _centerOfCube.name = "[CENTER OF CUBE]";
            _centerOfCube.transform.position = centerOfCubePos;
        }
    }

}

