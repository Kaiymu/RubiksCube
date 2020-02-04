using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private void Awake() {
        if (Instance == null)
            Instance = this;
        else {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(this);
    }

    public enum GAME_STATE { SCRAMBLED, PLAYING, WIN}
    public GAME_STATE gameState = GAME_STATE.SCRAMBLED;

    [Header("Number of cube to spawn X*X*X")]
    [Range(2, 6)]
    public int numberCube = 2;

    [Header("Number of random direction and positive that the cubes will do before starting the game")]
    public int numberRandomScramble = 10;

}

