using DG.Tweening;
using GravityThread.Core.Events;
using GravityThread.Services.Localization;
using UnityEngine;
using UnityEngine.UIElements;
using Zenject;

namespace GravityThread.UI
{
    [RequireComponent(typeof(UIDocument))]
    public sealed class HudView : MonoBehaviour
    {
        // Generated UI references (queried from UXML at runtime)
        private Label _scoreLabelText;
        private Label _scoreValueText;
        private Label _threadLabelText;
        private Label _threadPercentText;
        private Label _healthText;
        private VisualElement _threadBarFill;
        private Button _pauseButton;

        private EventBus _eventBus;
        private ILocalizationService _localization;

        private float _currentFillPercent = 1f;
        private Tweener _fillTween;
        private Tweener _punchTween;

        [Inject]
        public void Construct(EventBus eventBus, ILocalizationService localization)
        {
            _eventBus = eventBus;
            _localization = localization;
        }

        private void Awake()
        {
            var doc = GetComponent<UIDocument>();
            var root = doc.rootVisualElement;

            // Query all elements
            _scoreLabelText = root.Q<Label>("score-label");
            _scoreValueText = root.Q<Label>("score-value");
            _threadLabelText = root.Q<Label>("thread-label");
            _threadPercentText = root.Q<Label>("thread-percent");
            _healthText = root.Q<Label>("health-text");
            _threadBarFill = root.Q<VisualElement>("thread-bar-fill");
            _pauseButton = root.Q<Button>("pause-button");
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

            _fillTween?.Kill();
            _punchTween?.Kill();
        }

        private void OnScoreChanged(ScoreChangedEvent e)
        {
            if (_scoreValueText == null) return;

            _scoreValueText.text = e.Score.ToString();

            // Punch-scale animation via DOTween
            _punchTween?.Kill();
            _scoreValueText.transform.scale = Vector3.one;
            _punchTween = DOTween.To(
                    () => 0f,
                    t =>
                    {
                        // Simple punch: scale up then back
                        float s = 1f + 0.1f * Mathf.Sin(t * Mathf.PI);
                        _scoreValueText.transform.scale = new Vector3(s, s, 1f);
                    },
                    1f,
                    0.2f)
                .SetEase(Ease.OutQuad);
        }

        private void OnThreadEnergyChanged(ThreadEnergyChangedEvent e)
        {
            float target = Mathf.Clamp01(e.Normalized);

            if (_threadBarFill != null)
            {
                _fillTween?.Kill();
                _fillTween = DOTween.To(
                        () => _currentFillPercent,
                        val =>
                        {
                            _currentFillPercent = val;
                            // Scale width from 0%–100% of container, with 2px padding
                            _threadBarFill.style.right = new StyleLength(
                                Length.Percent((1f - val) * 100f));
                        },
                        target,
                        0.25f)
                    .SetEase(Ease.OutQuad);
            }

            if (_threadPercentText != null)
                _threadPercentText.text = Mathf.RoundToInt(target * 100f).ToString();
        }

        private void OnHealthChanged(BallHealthChangedEvent e)
        {
            if (_healthText != null)
                _healthText.text = $"{e.Current}/{e.Max}";
        }

        /// <summary>
        /// Access pause button for PauseView wiring.
        /// </summary>
        public Button PauseButton => _pauseButton;
    }
}