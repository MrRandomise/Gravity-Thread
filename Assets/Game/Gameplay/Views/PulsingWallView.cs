using UnityEngine;

namespace GravityThread.Gameplay.Views
{
    public sealed class PulsingWallView : MonoBehaviour
    {
        [SerializeField] private float _pulseFrequency = 2f;
        [SerializeField] private float _pulseAmplitude = 0.3f;
        [SerializeField] private Vector2 _pulseDirection = Vector2.right;

        private Vector3 _originalPosition;

        private void Awake()
        {
            _originalPosition = transform.localPosition;
        }

        private void Update()
        {
            float offset = Mathf.Sin(Time.time * _pulseFrequency * Mathf.PI * 2f) * _pulseAmplitude;
            transform.localPosition = _originalPosition + (Vector3)(_pulseDirection.normalized * offset);
        }

        public void SetPulseParams(float frequency, float amplitude, Vector2 direction)
        {
            _pulseFrequency = frequency;
            _pulseAmplitude = amplitude;
            _pulseDirection = direction;
        }
    }
}
