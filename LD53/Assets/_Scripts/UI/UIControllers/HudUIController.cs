﻿using LD53.Gameplay;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Util.GameEvents;
using Util.UI;
using Util.UI.Controllers;

namespace LD53.UI.UIControllers
{
    public class HudUIController : UIController
    {
        [Header("bruh")]
        [SerializeField] private Slider _timerSlider;
        [SerializeField] private TextMeshProUGUI _scoreText;
        [SerializeField] private UIPage _pauseMenu;

        [Header("Event Listeners")] 
        [SerializeField] private VoidGameEventSO _pauseGameEvent = default;
        [SerializeField] private IntGameEventSO _setScoreGameEvent = default;

        void Start()
        {
            _timerSlider.maxValue = GameManager.Instance.TimerStartDuration;
        }

        void OnEnable()
        {
            _pauseGameEvent.OnEventRaised += PauseGame;
            _setScoreGameEvent.OnEventRaised += SetScore;
        }

        void OnDisable()
        {
            _pauseGameEvent.OnEventRaised -= PauseGame;
            _setScoreGameEvent.OnEventRaised -= SetScore;
        }
        
        void Update()
        {
            _timerSlider.value = GameManager.Instance.TimerRemaining;
        }

        public void PauseGame()
        {
            _canvasController.DisplayUI(_pauseMenu);
        }

        public void ResumeGame()
        {
            _canvasController.HideUI(_pauseMenu);
        }

        public void SetScore(int score) => _scoreText.text = score.ToString();
    }
}