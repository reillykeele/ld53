using TMPro;
using UnityEngine;
using Util.UI.Controllers;
using GameManager = LD53.Gameplay.GameManager;

namespace LD53.UI.UIControllers
{
    public class ResultsUIController : UIController
    {
        [Header("Components")]
        [SerializeField] private TextMeshProUGUI _scoreText;

        void OnEnable()
        {
            SetScore(GameManager.Instance.Score);
        }

        public void SetScore(int score) => _scoreText.text = score.ToString();
    }
}