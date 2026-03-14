using DG.Tweening;
using GravityThread.Core.Events;
using GravityThread.Services.Localization;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using TMPro;

namespace GravityThread.UI
{
    public sealed class HudView : MonoBehaviour
    {
        [Header("Score")]
        [SerializeField] private TMP_Text _scoreText;

        [Header("Thread Energy")]
        [SerializeField] private Image _threadBar;

        [Header("Health")]
        [SerializeField] private TMP_Text _healthText;

        private EventBus _eventBus;
        private ILocalizationService _localization;

        [Inject]
        public void Construct(EventBus eventBus, ILocalizationService localization)
        {
            _eventBus = eventBus;
            _localization = localization;
        }

        private void OnEnable()
        {
            _eventBus.Subscribe<ScoreChangedEvent>(OnScoreChanged);
            _eventBus.Subscribe<ThreadEnergyChangedEvent>(OnThreadEnergyChanged);
            _eventBus.Subscribe<BallHealthChangedEvent>(OnHealthChanged);
        }

        private void OnDisable()
        {
            _eventBus.Unsubscribe<ScoreChangedEvent>(OnScoreChanged);
            _eventBus.Unsubscribe<ThreadEnergyChangedEvent>(OnThreadEnergyChanged);
            _eventBus.Unsubscribe<BallHealthChangedEvent>(OnHealthChanged);
        }

        private void OnScoreChanged(ScoreChangedEvent e)
        {
            if (_scoreText != null)
            {
                _scoreText.text = $"{_localization.Get("hud.score")}: {e.Score}";
                _scoreText.transform.DOKill();
                _scoreText.transform.DOPunchScale(Vector3.one * 0.1f, 0.2f);
            }
        }

        private void OnThreadEnergyChanged(ThreadEnergyChangedEvent e)
        {
            if (_threadBar != null)
                _threadBar.fillAmount = e.Normalized;
        }

        private void OnHealthChanged(BallHealthChangedEvent e)
        {
            if (_healthText != null)
                _healthText.text = $"{e.Current}/{e.Max}";
        }
    }
}
