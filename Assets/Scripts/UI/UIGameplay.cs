using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
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
        }

        public void Update()
        {
            string timeSpanFormatted = GameManager.Instance.TimerSecond.ToString("mm\\:ss");
            timerText.text = "Time : " + timeSpanFormatted;
        }
    }
}