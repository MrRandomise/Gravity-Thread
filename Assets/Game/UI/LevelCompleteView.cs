using DG.Tweening;
using GravityThread.Core;
using GravityThread.Core.Events;
using GravityThread.Gameplay;
using GravityThread.Services.Localization;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using TMPro;

namespace GravityThread.UI
{
    public sealed class LevelCompleteView : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private TMP_Text _titleText;
        [SerializeField] private TMP_Text _scoreText;
        [SerializeField] private TMP_Text _bonusText;
        [SerializeField] private Button _nextButton;
        [SerializeField] private Button _menuButton;

        private EventBus _eventBus;
        private ILocalizationService _localization;
        private GameStateManager _stateManager;
        private LevelFlowManager _levelFlow;

        [Inject]
        public void Construct(EventBus eventBus, ILocalizationService localization,
            GameStateManager stateManager, LevelFlowManager levelFlow)
        {
            _eventBus = eventBus;
            _localization = localization;
            _stateManager = stateManager;
            _levelFlow = levelFlow;
        }

        private void Awake()
        {
            if (_canvasGroup != null)
            {
                _canvasGroup.alpha = 0;
                _canvasGroup.interactable = false;
                _canvasGroup.blocksRaycasts = false;
            }
        }

        private void OnEnable()
        {
            _eventBus.Subscribe<LevelCompletedEvent>(OnLevelCompleted);
            if (_nextButton != null) _nextButton.onClick.AddListener(OnNextClicked);
            if (_menuButton != null) _menuButton.onClick.AddListener(OnMenuClicked);
        }

        private void OnDisable()
        {
            _eventBus.Unsubscribe<LevelCompletedEvent>(OnLevelCompleted);
            if (_nextButton != null) _nextButton.onClick.RemoveListener(OnNextClicked);
            if (_menuButton != null) _menuButton.onClick.RemoveListener(OnMenuClicked);
        }

        private void OnLevelCompleted(LevelCompletedEvent e)
        {
            if (_titleText != null)
                _titleText.text = _localization.Get("level.complete");
            if (_scoreText != null)
                _scoreText.text = _localization.Get("level.score", e.Score);

            string bonus = "";
            if (e.NoCollisions) bonus += _localization.Get("level.bonus.nocollision") + "\n";
            if (e.EnergyUsedPercent < 0.2f) bonus += _localization.Get("level.bonus.lowenergy");
            if (_bonusText != null)
                _bonusText.text = bonus;

            Show();
        }

        private void Show()
        {
            if (_canvasGroup == null) return;
            _canvasGroup.interactable = true;
            _canvasGroup.blocksRaycasts = true;
            DOTween.To(() => _canvasGroup.alpha, x => _canvasGroup.alpha = x, 1f, 0.4f).SetUpdate(true);
        }

        private void Hide()
        {
            if (_canvasGroup == null) return;
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
            DOTween.To(() => _canvasGroup.alpha, x => _canvasGroup.alpha = x, 0f, 0.3f).SetUpdate(true);
        }

        private void OnNextClicked()
        {
            Hide();
            _levelFlow.LoadNextLevel();
            _stateManager.SetState(GameState.Playing);
        }

        private void OnMenuClicked()
        {
            Hide();
            _stateManager.SetState(GameState.MainMenu);
        }
    }
}
