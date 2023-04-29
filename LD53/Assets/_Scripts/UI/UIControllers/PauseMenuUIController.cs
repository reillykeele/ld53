using LD53.Gameplay;
using UnityEngine;
using UnityEngine.UI;
using Util.GameEvents;
using Util.UI;
using Util.UI.Controllers;

namespace LD53.UI.UIControllers
{
    public class PauseMenuUIController : UIController
    {
        [Header("Event Listeners")]
        [SerializeField] private VoidGameEventSO _resumeGameEvent = default;
        
        void OnEnable()
        {
            _resumeGameEvent.OnEventRaised += ResumeGame;
        }

        void OnDisable()
        {
            _resumeGameEvent.OnEventRaised -= ResumeGame;
        }

        public void ResumeGame()
        {
            _canvasController.HideUI(Page);
        }
    }
}