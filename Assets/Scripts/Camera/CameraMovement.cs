using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [Header("Min value to reach to move")]
    public Vector2 miniMalValueDir = new Vector2(0.3f, 0.3f);
    [Header("Speed of the movement")]
    public float speedMovement = 30f;

    private Transform _rotateAround;

    private void Start() {
        _rotateAround = GameManager.Instance.CenterOfCube.transform;
    }

    private void Update() {
        if (GameManager.Instance.gameState == GameManager.GAME_STATE.WIN)
        {

        }
        else
        {

            transform.RotateAround(_rotateAround.transform.position, InputManager.Instance._RotateMobile(miniMalValueDir), Time.deltaTime * speedMovement);
        }
    }
}
