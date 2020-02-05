using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Rubiks.UI
{
    public class UIGameplay : MonoBehaviour
    {
        [Header("Columns")]
        public Button goPositiveColumn;
        public Button goNegativeColumn;

        [Header("Rows")]
        public Button goPositiveRow;
        public Button goNegativeRow;

        [Header("Depth")]
        public Button goPositiveDepth;
        public Button goNegativeDepth;

        [Header("Undo button")]
        public Button undoButton;

        [Header("Timer ")]
        public TextMeshProUGUI timerText;

        [Header("Main menu")]
        public Button[] restartGame;
        public Button[] goBackMainMenu;
        public Toggle hideShowTimer;
        public Button[] closeOpenMainMenu;
        public RectTransform containerMainMenu;

        [Header("Win menu")]
        public TextMeshProUGUI congratulationsText;
        public Button closeWinMenu;
        public RectTransform containerWinMenu;

        private void Awake()
        {
            goPositiveColumn.onClick.AddListener(() =>
            {
                CubeManager.Instance.Rotate(Vector3.right);
            });

            goNegativeColumn.onClick.AddListener(() =>
            {
                CubeManager.Instance.Rotate(Vector3.left);
            });

            goPositiveRow.onClick.AddListener(() =>
            {
                CubeManager.Instance.Rotate(Vector3.up);
            });

            goNegativeRow.onClick.AddListener(() =>
            {
                CubeManager.Instance.Rotate(Vector3.down);
            });

            goPositiveDepth.onClick.AddListener(() =>
            {
                CubeManager.Instance.Rotate(Vector3.forward);
            });

            goNegativeDepth.onClick.AddListener(() =>
            {
                CubeManager.Instance.Rotate(Vector3.back);
            });

            undoButton.onClick.AddListener(() =>
            {
                CubeManager.Instance.UndoLastRotation();
            });

            for (int i = 0; i < restartGame.Length; i++) {
                restartGame[i].onClick.AddListener(() =>
                {
                    if (DatasManager.Instance != null)
                    {
                        DatasManager.Instance.cubeSaveContainer.timerInSeconds = 0f;
                        DatasManager.Instance.cubeSaveContainer.miniCubeSaveList.Clear();
                    }
                    StartNewGame(GameManager.Instance.numberCube);
                });
            }

            for (int i = 0; i < goBackMainMenu.Length; i++)
            {
                goBackMainMenu[i].onClick.AddListener(() =>
                {
                    SceneManager.LoadScene(0);
                });
            }

            for (int i = 0; i < closeOpenMainMenu.Length; i++) {
                closeOpenMainMenu[i].onClick.AddListener(() =>
                {
                    containerMainMenu.gameObject.SetActive(!containerMainMenu.gameObject.activeInHierarchy);
                });
            }

            closeWinMenu.onClick.AddListener(() =>
            {
                containerWinMenu.gameObject.SetActive(!containerWinMenu.gameObject.activeInHierarchy);
            });
        }

        public void Update()
        {
            var currentState = GameManager.Instance.gameState;
            if (currentState == GameManager.GAME_STATE.PLAYING)
            {
                string timeSpanFormatted = GameManager.Instance.TimerSecond.ToString("mm\\:ss");
                timerText.text = "Time : " + timeSpanFormatted;
            }
            else if (currentState == GameManager.GAME_STATE.WIN)
            {
                string timeSpanFormatted = GameManager.Instance.TimerSecond.ToString("mm\\:ss");
                congratulationsText.text = "Congratulations ! You beat the game in : " + timeSpanFormatted;
                containerWinMenu.gameObject.SetActive(true);
            }
        }

        public void StartNewGame(int numberCube)
        {
            GameManager.Instance.numberCube = numberCube;
            SceneManager.LoadScene(1);
        }
    }
}