using DG.Tweening;
using GravityThread.Core;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using UIToolkitButton = UnityEngine.UIElements.Button;

namespace GravityThread.UI
{
    public sealed class PauseView : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _pausePanel;
        [SerializeField] private Button _resumeButton;

        private GameStateManager _stateManager;
        private UIToolkitButton _pauseButton;

        [Inject]
        public void Construct(GameStateManager stateManager)
        {
            _stateManager = stateManager;
        }

        private void Awake()
        {
            if (_pausePanel != null)
            {
                _pausePanel.alpha = 0;
                _pausePanel.interactable = false;
                _pausePanel.blocksRaycasts = false;
            }
        }

        private void Start()
        {
            // Get pause button from HudView (built at runtime in Awake)
            var hudView = GetComponent<HudView>();
            if (hudView != null)
                _pauseButton = hudView.PauseButton;

            // UI Toolkit Button uses clicked event instead of onClick
            if (_pauseButton != null)
                _pauseButton.clicked += OnPauseClicked;
        }

        private void OnEnable()
        {
            if (_resumeButton != null) _resumeButton.onClick.AddListener(OnResumeClicked);
        }

        private void OnDisable()
        {
            if (_pauseButton != null) _pauseButton.clicked -= OnPauseClicked;
            if (_resumeButton != null) _resumeButton.onClick.RemoveListener(OnResumeClicked);
        }

        private void OnPauseClicked()
        {
            _stateManager.SetState(GameState.Paused);
            if (_pausePanel != null)
            {
                _pausePanel.interactable = true;
                _pausePanel.blocksRaycasts = true;
                FadeCanvasGroup(_pausePanel, 1f, 0.3f);
            }
        }

        private void OnResumeClicked()
        {
            _stateManager.SetState(GameState.Playing);
            if (_pausePanel != null)
            {
                _pausePanel.interactable = false;
                _pausePanel.blocksRaycasts = false;
                FadeCanvasGroup(_pausePanel, 0f, 0.3f);
            }
        }

        private static void FadeCanvasGroup(CanvasGroup cg, float target, float duration)
        {
            DOTween.To(() => cg.alpha, x => cg.alpha = x, target, duration).SetUpdate(true);
        }
    }
}
