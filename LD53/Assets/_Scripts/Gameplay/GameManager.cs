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
        [SerializeField] private VoidGameEventSO _timesUpGameEvent;

        [SerializeField] public float TimerStartDuration;
        [SerializeField, ReadOnly] public float TimerRemaining;

        void Start()
        {
            TimerRemaining = TimerStartDuration;

            GameSystem.Instance.ChangeGameState(GameState.Playing);
        }

        void Update()
        {
            TimerRemaining -= Time.deltaTime;

            if (TimerRemaining <= 0)
            {
                // game over
                _timesUpGameEvent.RaiseEvent();
            }
        }

        public int GetHighestSortingOrder()
        {
            // TODO: for the love of god figure out a better way to do this
            var mails = FindObjectsOfType<Mail>();

            int maxSortOrder = 0;
            foreach (var mail in mails)
            {
                if (mail.SortOrder == Int16.MaxValue) 
                    continue;

                maxSortOrder = Mathf.Max(maxSortOrder, mail.SortOrder);
            }

            return maxSortOrder;
        }
    }
}