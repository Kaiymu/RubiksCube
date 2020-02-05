using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIMenu : MonoBehaviour
{
    public Button quitGame;
    private void Awake()
    {
        quitGame.onClick.AddListener(() =>
        {
            Application.Quit();
        });
    }

    public void StartNewGame(int numberCube)
    {
        GameManager.Instance.numberCube = numberCube;
        SceneManager.LoadScene(1);
    }

    public void LoadLevel(int numberCube)
    {
        GameManager.Instance.numberCube = numberCube;
        SceneManager.LoadScene(1);
    }
}
