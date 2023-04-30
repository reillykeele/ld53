using System;
using LD53.Input;
using UnityEngine;
using Util.Attributes;
using Util.Coroutine;
using Util.Enums;
using Util.GameEvents;
using Util.Singleton;
using Util.Systems;

namespace LD53.Gameplay
{
    public class GameManager : Singleton<GameManager>
    {
        [Header("Event Dispatchers")]
        [SerializeField] private IntGameEventSO _setScoreGameEvent;
        [SerializeField] private VoidGameEventSO _timesUpGameEvent;
        [SerializeField] private VoidGameEventSO _startMailGameEvent;

        [Header("Event Listeners")] 
        [SerializeField] private VoidGameEventSO _sceneLoadedGameEvent;

        [Header("config")] 
        [SerializeField] private float _startDelayTime = 1f;
        [SerializeField, ReadOnly] private bool _isTimerStarted = false;
        [SerializeField] public float TimerStartDuration;
        [SerializeField, ReadOnly] public float TimerRemaining;

        [SerializeField, ReadOnly] public int Score;

        private Camera _camera;

        void Start()
        {
            TimerRemaining = TimerStartDuration;

            GameSystem.Instance.ChangeGameState(GameState.Cutscene);

            _camera = Camera.main;
        }

        void OnEnable()
        {
            _sceneLoadedGameEvent.OnEventRaised += StartMailCountdown;
        }

        void OnDisable()
        {
            _sceneLoadedGameEvent.OnEventRaised -= StartMailCountdown;
        }

        void Update()
        {
            if (GameSystem.Instance.IsPlaying() == false) return;

            if (_isTimerStarted)
            {
                TimerRemaining -= Time.deltaTime;

                if (TimerRemaining <= 0)
                {
                    TimesUp();
                }
            }
        }

        private void StartMailCountdown()
        {
            StartCoroutine(CoroutineUtil.Sequence(
                new WaitForSecondsRealtime(_startDelayTime),
                CoroutineUtil.CallAction(StartMail)
            ));
        }

        private void StartMail()
        {
            _startMailGameEvent.RaiseEvent();
            
            GameSystem.Instance.ChangeGameState(GameState.Playing);

            _isTimerStarted = true;
        }

        private void TimesUp()
        {
            _timesUpGameEvent.RaiseEvent();

            GameSystem.Instance.ChangeGameState(GameState.Cutscene);

            _isTimerStarted = false;
        }

        public float GetClippingPlaneZ() => _camera.transform.position.z + _camera.nearClipPlane;

        public float GetClosestZ()
        {
            // TODO: for the love of god figure out a better way to do this
            var mails = FindObjectsOfType<Mail>();

            var minZ = 0f;
            var clippingPlaneZ = GetClippingPlaneZ() + 0.15f;
            foreach (var mail in mails)
            {
                if (mail.transform.position.z <= clippingPlaneZ)
                    continue;

                minZ = Mathf.Min(minZ, mail.transform.position.z);
            }

            return minZ;
        }

        public float GetTopZ() => GetClosestZ() - 0.025f;

        public void AddScore(int points)
        {
            Score += points;
            _setScoreGameEvent.RaiseEvent(Score);
        }
    }
}