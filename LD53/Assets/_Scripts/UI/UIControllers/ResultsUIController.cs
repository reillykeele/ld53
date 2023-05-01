using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Util.UI.Controllers;
using GameManager = LD53.Gameplay.GameManager;

namespace LD53.UI.UIControllers
{
    public class ResultsUIController : UIController
    {
        [Header("Components")]
        [SerializeField] private TextMeshProUGUI _scoreText;
        [SerializeField] private Image _star1;
        [SerializeField] private Image _star2;
        [SerializeField] private Image _star3;
        [SerializeField] private Color _litStarColor;

        [Header("Config")] 
        [SerializeField, Min(0)] private int _minScoreStar1;
        [SerializeField, Min(0)] private int _minScoreStar2;
        [SerializeField, Min(0)] private int _minScoreStar3;

        void OnEnable()
        {
            SetScore(GameManager.Instance.Score);
            SetStars(GameManager.Instance.Score);
        }

        public void SetScore(int score) => _scoreText.text = score.ToString();

        public void SetStars(int score)
        {
            _star1.color = score >= _minScoreStar1 ? _litStarColor : Color.black;
            _star2.color = score >= _minScoreStar2 ? _litStarColor : Color.black;
            _star3.color = score >= _minScoreStar3 ? _litStarColor : Color.black;
        }
    }
}