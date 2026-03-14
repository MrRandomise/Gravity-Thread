using GravityThread.Gameplay;
using GravityThread.Services.Input;
using UnityEngine;
using Zenject;

namespace GravityThread.Gameplay.Views
{
    public sealed class ThreadFieldView : MonoBehaviour
    {
        [SerializeField] private LineRenderer _lineRenderer;
        [SerializeField] private ParticleSystem _fieldParticles;
        [SerializeField] private int _lineSegments = 20;
        [SerializeField] private float _waveAmplitude = 0.15f;
        [SerializeField] private float _waveFrequency = 4f;

        private ThreadSystem _thread;
        private BallController _ball;
        private IInputService _input;

        [Inject]
        public void Construct(ThreadSystem thread, BallController ball, IInputService input)
        {
            _thread = thread;
            _ball = ball;
            _input = input;
        }

        private void Awake()
        {
            if (_lineRenderer != null)
            {
                _lineRenderer.positionCount = _lineSegments;
                _lineRenderer.enabled = false;
            }
        }

        private void Update()
        {
            if (!_thread.IsActive || !_ball.IsAlive)
            {
                if (_lineRenderer != null) _lineRenderer.enabled = false;
                if (_fieldParticles != null && _fieldParticles.isPlaying) _fieldParticles.Stop();
                return;
            }

            Vector2 ballPos = _ball.Position;
            Vector2 touchPos = _input.TouchWorldPosition;

            if (_lineRenderer != null)
            {
                _lineRenderer.enabled = true;
                UpdateLine(ballPos, touchPos);
            }

            if (_fieldParticles != null)
            {
                _fieldParticles.transform.position = touchPos;
                if (!_fieldParticles.isPlaying) _fieldParticles.Play();
            }
        }

        private void UpdateLine(Vector2 from, Vector2 to)
        {
            Vector2 direction = to - from;
            Vector2 perpendicular = new Vector2(-direction.y, direction.x).normalized;

            for (int i = 0; i < _lineSegments; i++)
            {
                float t = (float)i / (_lineSegments - 1);
                Vector2 basePoint = Vector2.Lerp(from, to, t);

                float wave = Mathf.Sin(t * _waveFrequency * Mathf.PI * 2f + Time.time * 8f) * _waveAmplitude;
                wave *= Mathf.Sin(t * Mathf.PI); // Fade at endpoints

                Vector2 point = basePoint + perpendicular * wave;
                _lineRenderer.SetPosition(i, new Vector3(point.x, point.y, 0));
            }

            float energyAlpha = _thread.NormalizedEnergy;
            Color startColor = _lineRenderer.startColor;
            startColor.a = energyAlpha;
            _lineRenderer.startColor = startColor;
            Color endColor = _lineRenderer.endColor;
            endColor.a = energyAlpha * 0.5f;
            _lineRenderer.endColor = endColor;
        }
    }
}
