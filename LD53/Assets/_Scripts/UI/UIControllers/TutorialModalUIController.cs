using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Util.Helpers;
using Util.UI.Controllers;

namespace LD53.UI.UIControllers
{
    internal class TutorialModalUIController : UIController
    {
        [Header("Modal Components")]
        [SerializeField] private TextMeshProUGUI _title;
        [SerializeField] private GameObject _content;
        [SerializeField] private Button _nextButton;
        [SerializeField] private TextMeshProUGUI _nextButtonText;
        [SerializeField] private Button _backButton;
        [SerializeField] private TextMeshProUGUI _backButtonText;

        [SerializeField] private UnityEvent _defaultNextEvent = new UnityEvent();
        [SerializeField] private UnityEvent _defaultBackEvent = new UnityEvent();

        private Action _nextAction;
        private Action _backAction;

        void OnEnable()
        {
            _nextButton.onClick.AddListener(OnYes);
            _backButton.onClick.AddListener(OnNo);
        }

        void OnDisable()
        {
            _nextButton.onClick.RemoveListener(OnYes);
            _backButton.onClick.RemoveListener(OnNo);
        }

        public void DisplayModal(string title, Action nextAction = null, Action backAction = null, string nextButtonText = "Continue", string backButtonText = "Back")
        {
            if (title.IsNullOrEmpty() == false)
                _title.text = title;
            else
                _title.gameObject.Disable();

            if (nextButtonText.IsNullOrEmpty() == false)
                _nextButtonText.text = nextButtonText;
            else
                _nextButton.gameObject.Disable();
            
            if (backButtonText.IsNullOrEmpty() == false)
                _backButtonText.text = backButtonText;
            else 
                _backButton.gameObject.Disable();

            _nextAction = nextAction ?? _defaultNextEvent.Invoke;
            _backAction = _defaultBackEvent.Invoke;

            OpenModal();
        }

        public void OpenModal() => _canvasController.DisplayUI(Page);

        public void CloseModal() => _canvasController.HideUI(Page);

        private void OnYes() => _nextAction?.Invoke();
        private void OnNo() => _backAction?.Invoke();
    }
}
