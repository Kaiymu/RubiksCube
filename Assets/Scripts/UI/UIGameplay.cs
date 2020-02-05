using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Rubiks.UI
{
    public class UIGameplay : MonoBehaviour {
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
        public Button restartGame;
        public Button goBackMainMenu;
        public Toggle hideShowTimer;
        public Button[] closeOpenMainMenu;
        public RectTransform containerMainMenu;

        [Header("Win menu")]
        public Button restartGameWin;
        public Button goBackMainMenuWin;
        public TextMeshProUGUI congratulationsText;
        public Button closeWinMenu;
        public RectTransform containerWinMenu;

        [Header("Dialogue validate quit")]
        public Button yesQuit;
        public Button[] closeMenu;
        public RectTransform containerValidateQuit;

        // Quite ugly ... !
        private bool _alreadyWon = false;

        #region ENABLE
        private void OnEnable() {
            _alreadyWon = false;
            goPositiveColumn.onClick.AddListener(() => {
                CubeManager.Instance.Rotate(Vector3.right);
            });

            goNegativeColumn.onClick.AddListener(() => {
                CubeManager.Instance.Rotate(Vector3.left);
            });

            goPositiveRow.onClick.AddListener(() => {
                CubeManager.Instance.Rotate(Vector3.up);
            });

            goNegativeRow.onClick.AddListener(() => {
                CubeManager.Instance.Rotate(Vector3.down);
            });

            goPositiveDepth.onClick.AddListener(() => {
                CubeManager.Instance.Rotate(Vector3.forward);
            });

            goNegativeDepth.onClick.AddListener(() => {
                CubeManager.Instance.Rotate(Vector3.back);
            });

            undoButton.onClick.AddListener(() => {
                CubeManager.Instance.UndoLastRotation();
            });

            // Main menu
            restartGame.onClick.AddListener(() => {
                containerValidateQuit.gameObject.SetActive(true);
                yesQuit.onClick.RemoveAllListeners();

                // Validate restart game
                yesQuit.onClick.AddListener(() => {
                    _RestartLevel();
                });
            });

            goBackMainMenu.onClick.AddListener(() => {
                containerValidateQuit.gameObject.SetActive(true);
                yesQuit.onClick.RemoveAllListeners();
                // Dialogue validate quit
                yesQuit.onClick.AddListener(() => {
                    _GoBackMainMenu();
                });
            });

            hideShowTimer.isOn = true;
            hideShowTimer.onValueChanged.AddListener((hideOrShow) => {
                timerText.gameObject.transform.parent.gameObject.SetActive(hideOrShow);
            });

            for (int i = 0; i < closeOpenMainMenu.Length; i++) {
                closeOpenMainMenu[i].onClick.AddListener(() => {
                    containerMainMenu.gameObject.SetActive(!containerMainMenu.gameObject.activeInHierarchy);
                });
            }

            // Win menu
            restartGameWin.onClick.AddListener(() => {
                _RestartLevel();
            });

            goBackMainMenuWin.onClick.AddListener(() => {
                _GoBackMainMenu();
            });


            closeWinMenu.onClick.AddListener(() => {
                containerWinMenu.gameObject.SetActive(false);
            });

            for (int i = 0; i < closeMenu.Length; i++) {
                closeMenu[i].onClick.AddListener(() => {
                    yesQuit.onClick.RemoveAllListeners();
                    containerValidateQuit.gameObject.SetActive(false);
                });
            }
        }

        #endregion

        #region DISABLE
        private void OnDisable() {
            goPositiveColumn.onClick.RemoveAllListeners();

            goNegativeColumn.onClick.RemoveAllListeners();

            goPositiveRow.onClick.RemoveAllListeners();

            goNegativeRow.onClick.RemoveAllListeners();

            goPositiveDepth.onClick.RemoveAllListeners();

            goNegativeDepth.onClick.RemoveAllListeners();

            undoButton.onClick.RemoveAllListeners();

            // Main menu
            restartGame.onClick.RemoveAllListeners();

            goBackMainMenu.onClick.RemoveAllListeners();

            hideShowTimer.onValueChanged.RemoveAllListeners();

            for (int i = 0; i < closeOpenMainMenu.Length; i++) {
                closeOpenMainMenu[i].onClick.RemoveAllListeners();
            }

            // Win menu
            restartGameWin.onClick.RemoveAllListeners();

            goBackMainMenuWin.onClick.RemoveAllListeners();

            closeWinMenu.onClick.RemoveAllListeners();

            for (int i = 0; i < closeMenu.Length; i++) {
                closeMenu[i].onClick.RemoveAllListeners();
            }
            
            yesQuit.onClick.RemoveAllListeners();
        }
        #endregion

        private void _RestartLevel() {
            if (DatasManager.Instance != null) {
                DatasManager.Instance.cubeSaveContainer.timerInSeconds = 0f;
                DatasManager.Instance.cubeSaveContainer.miniCubeSaveList.Clear();
            }
            StartNewGame(GameManager.Instance.numberCube);
        }

        private void _GoBackMainMenu() {
            SceneManager.LoadScene(0);
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
                if (!_alreadyWon) {
                    _alreadyWon = true;
                    containerWinMenu.gameObject.SetActive(true);
                }
            }
        }

        public void StartNewGame(int numberCube)
        {
            GameManager.Instance.numberCube = numberCube;
            SceneManager.LoadScene(1);
        }
    }
}