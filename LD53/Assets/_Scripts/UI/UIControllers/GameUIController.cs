using LD53.Gameplay;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Util.Coroutine;
using Util.Enums;
using Util.GameEvents;
using Util.Helpers;
using Util.UI;
using Util.UI.Controllers;
using Coroutine = UnityEngine.Coroutine;

namespace LD53.UI.UIControllers
{
    public class GameUIController : UIController
    {
        [Header("Components")]
        [SerializeField] private Slider _timerSlider;
        [SerializeField] private TextMeshProUGUI _scoreText;
        [SerializeField] private TextMeshProUGUI _startText;
        [SerializeField] private TextMeshProUGUI _timesUpText;
        [SerializeField] private float _resultPageDelay = 5f;

        [Header("UI Pages")]
        [SerializeField] private UIPage _pauseMenu;
        [SerializeField] private UIPage _results;

        [Header("Event Listeners")] 
        [SerializeField] private VoidGameEventSO _pauseGameEvent = default;
        [SerializeField] private IntGameEventSO _setScoreGameEvent = default;
        [SerializeField] private VoidGameEventSO _startMailGameEvent = default;
        [SerializeField] private VoidGameEventSO _timesUpGameEvent = default;

        void Start()
        {
            _timerSlider.maxValue = GameManager.Instance.TimerStartDuration;
        }

        void OnEnable()
        {
            _pauseGameEvent.OnEventRaised += PauseGame;
            _setScoreGameEvent.OnEventRaised += SetScore;
            _startMailGameEvent.OnEventRaised += StartMail;
            _timesUpGameEvent.OnEventRaised += TimesUp;
        }

        void OnDisable()
        {
            _pauseGameEvent.OnEventRaised -= PauseGame;
            _setScoreGameEvent.OnEventRaised -= SetScore;
            _startMailGameEvent.OnEventRaised -= StartMail;
            _timesUpGameEvent.OnEventRaised -= TimesUp;
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

        public void StartMail()
        {
            _startText.gameObject.Enable();
        }

        public void TimesUp()
        {
            _timesUpText.gameObject.Enable();

            StartCoroutine(CoroutineUtil.WaitForExecute(() => _canvasController.SwitchUI(_results), _resultPageDelay));
        }
    }
}