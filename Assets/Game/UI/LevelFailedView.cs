using DG.Tweening;
using GravityThread.Core;
using GravityThread.Core.Events;
using GravityThread.Services.Localization;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using TMPro;

namespace GravityThread.UI
{
    public sealed class LevelFailedView : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private TMP_Text _titleText;
        [SerializeField] private Button _retryButton;
        [SerializeField] private Button _menuButton;

        private EventBus _eventBus;
        private ILocalizationService _localization;
        private GameStateManager _stateManager;
        private Gameplay.LevelFlowManager _levelFlow;

        [Inject]
        public void Construct(EventBus eventBus, ILocalizationService localization,
            GameStateManager stateManager, Gameplay.LevelFlowManager levelFlow)
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
            _eventBus.Subscribe<LevelFailedEvent>(OnLevelFailed);
            if (_retryButton != null) _retryButton.onClick.AddListener(OnRetryClicked);
            if (_menuButton != null) _menuButton.onClick.AddListener(OnMenuClicked);
        }

        private void OnDisable()
        {
            _eventBus.Unsubscribe<LevelFailedEvent>(OnLevelFailed);
            if (_retryButton != null) _retryButton.onClick.RemoveListener(OnRetryClicked);
            if (_menuButton != null) _menuButton.onClick.RemoveListener(OnMenuClicked);
        }

        private void OnLevelFailed(LevelFailedEvent e)
        {
            if (_titleText != null)
                _titleText.text = _localization.Get("level.failed");
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

        private void OnRetryClicked()
        {
            Hide();
            _levelFlow.LoadLevel(_levelFlow.CurrentLevelIndex);
            _stateManager.SetState(GameState.Playing);
        }

        private void OnMenuClicked()
        {
            Hide();
            _stateManager.SetState(GameState.MainMenu);
        }
    }
}
