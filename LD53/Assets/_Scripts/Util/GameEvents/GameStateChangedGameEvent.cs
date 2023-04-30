using UnityEngine;
using UnityEngine.Events;
using Util.Enums;

namespace Util.GameEvents
{
    [CreateAssetMenu(fileName = "GameStateChangedGameEvent", menuName = "Game Event/Game State Changed Game Event")]
    public class GameStateChangedGameEventSO : ScriptableObject
    {
        public UnityAction<GameState> OnEventRaised;

        public void RaiseEvent(GameState value) => OnEventRaised?.Invoke(value);
    }
}