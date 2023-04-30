using System;
using LD53.Input;
using UnityEngine;
using Util.Attributes;
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

        [Header("config")]
        [SerializeField] public float TimerStartDuration;
        [SerializeField, ReadOnly] public float TimerRemaining;

        [SerializeField, ReadOnly] private int _score;

        private Camera _camera;

        void Start()
        {
            TimerRemaining = TimerStartDuration;

            GameSystem.Instance.ChangeGameState(GameState.Playing);

            _camera = Camera.main;
        }

        void Update()
        {
            if (GameSystem.Instance.IsPlaying() == false) return;

            TimerRemaining -= Time.deltaTime;

            if (TimerRemaining <= 0)
            {
                // game over
                _timesUpGameEvent.RaiseEvent();
            }
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
            _score += points;
            _setScoreGameEvent.RaiseEvent(_score);
        }
    }
}