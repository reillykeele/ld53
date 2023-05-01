using LD53.Gameplay;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Util.Audio;
using Util.Coroutine;
using Util.Enums;
using Util.GameEvents;
using Util.Helpers;
using Util.Systems;
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
        [SerializeField] private VoidGameEventSO _resumeGameEvent = default;
        [SerializeField] private IntGameEventSO _setScoreGameEvent = default;
        [SerializeField] private VoidGameEventSO _startMailGameEvent = default;
        [SerializeField] private VoidGameEventSO _timesUpGameEvent = default;

        [Header("Audio")]
        [SerializeField] private AudioSoundSO _music;
        [SerializeField] private AudioSoundSO _startSound;
        [SerializeField] private AudioSoundSO _stopSound;

        void Start()
        {
            _timerSlider.maxValue = GameManager.Instance.TimerStartDuration;
        }

        void OnEnable()
        {
            _pauseGameEvent.OnEventRaised += PauseGame;
            _resumeGameEvent.OnEventRaised += ResumeGame;
            _setScoreGameEvent.OnEventRaised += SetScore;
            _startMailGameEvent.OnEventRaised += StartMail;
            _timesUpGameEvent.OnEventRaised += TimesUp;
        }

        void OnDisable()
        {
            _pauseGameEvent.OnEventRaised -= PauseGame;
            _resumeGameEvent.OnEventRaised += ResumeGame;
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

            AudioSystem.Instance.PauseAudioStream(_music.AudioStream);
        }

        public void ResumeGame()
        {
            // _canvasController.HideUI(_pauseMenu);

            AudioSystem.Instance.UnpauseAudioStream(_music.AudioStream);
        }

        public void SetScore(int score) => _scoreText.text = score.ToString();

        public void StartMail()
        {
            _startText.gameObject.Enable();

            AudioSystem.Instance.PlayOnAudioStream(_music);
            AudioSystem.Instance.PlayAudioSound(_startSound);
        }

        public void TimesUp()
        {
            _timesUpText.gameObject.Enable();

            AudioSystem.Instance.StopAudioStream(_music.AudioStream);
            AudioSystem.Instance.PlayAudioSound(_stopSound);

            StartCoroutine(CoroutineUtil.WaitForExecute(() => _canvasController.SwitchUI(_results), _resultPageDelay));
        }
    }
}