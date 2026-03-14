using GravityThread.Configs;
using GravityThread.Core.Events;
using GravityThread.Core.Interfaces;
using GravityThread.Services.Debug;
using UnityEngine;

namespace GravityThread.Gameplay
{
    public sealed class BallController : IGameFixedTickable, IGameLateTickable
    {
        private readonly BallConfig _config;
        private readonly ThreadSystem _threadSystem;
        private readonly EventBus _eventBus;
        private readonly IDebugService _debug;

        private Rigidbody2D _rigidbody;
        private Transform _transform;
        private TrailRenderer _trail;

        private int _currentHealth;
        private float _currentScale;
        private Color _currentColor;
        private bool _hadCollisionThisLevel;
        private bool _isAlive;

        public bool IsAlive => _isAlive;
        public bool HadCollision => _hadCollisionThisLevel;
        public Vector2 Position => _rigidbody != null ? _rigidbody.position : Vector2.zero;
        public float Speed => _rigidbody != null ? _rigidbody.linearVelocity.magnitude : 0f;
        public Color CurrentColor => _currentColor;

        public BallController(BallConfig config, ThreadSystem threadSystem, EventBus eventBus, IDebugService debug)
        {
            _config = config;
            _threadSystem = threadSystem;
            _eventBus = eventBus;
            _debug = debug;
        }

        public void Initialize(Rigidbody2D rigidbody, TrailRenderer trail)
        {
            _rigidbody = rigidbody;
            _transform = rigidbody.transform;
            _trail = trail;

            _rigidbody.mass = _config.Mass;
            _currentHealth = _config.MaxHealth;
            _currentScale = _config.DefaultScale;
            _currentColor = Color.white;
            _isAlive = true;
            _hadCollisionThisLevel = false;

            ApplyScale();
            _eventBus.Publish(new BallHealthChangedEvent { Current = _currentHealth, Max = _config.MaxHealth });
        }

        public void GameFixedTick(float fixedDeltaTime)
        {
            if (!_isAlive || _rigidbody == null) return;

            Vector2 force = _threadSystem.CalculateForce(_rigidbody.position);
            if (force.sqrMagnitude > 0.01f)
                _rigidbody.AddForce(force, ForceMode2D.Force);

            if (_rigidbody.linearVelocity.magnitude > _config.MaxSpeed)
                _rigidbody.linearVelocity = _rigidbody.linearVelocity.normalized * _config.MaxSpeed;
        }

        public void GameLateTick(float deltaTime)
        {
            if (!_isAlive || _trail == null) return;

            float speedRatio = Mathf.Clamp01(Speed / _config.MaxSpeed);
            float width = _config.BaseTrailWidth + (_config.TrailWidthMultiplierAtMaxSpeed - 1f) * _config.BaseTrailWidth * speedRatio;
            _trail.startWidth = width;
            _trail.endWidth = width * 0.1f;
            _trail.time = _config.TrailLifetime * (0.5f + speedRatio * 0.5f);
        }

        public void OnWallCollision(float impactForce)
        {
            _hadCollisionThisLevel = true;
            _eventBus.Publish(new BallWallCollisionEvent { ImpactForce = impactForce });

            if (impactForce > _config.WallDamageForceThreshold)
            {
                _currentHealth--;
                _eventBus.Publish(new BallHealthChangedEvent { Current = _currentHealth, Max = _config.MaxHealth });
                _debug.Log($"Ball hit wall hard! Health: {_currentHealth}");

                if (_currentHealth <= 0)
                {
                    _isAlive = false;
                    _eventBus.Publish(new BallDestroyedEvent());
                }
            }
        }

        public void OnSpikeHit(Vector3 position)
        {
            _currentScale = Mathf.Max(_config.MinScale, _currentScale - _config.SizeChangePerSpikeHit);
            ApplyScale();
            _eventBus.Publish(new SpikeHitEvent { Position = position });
            _eventBus.Publish(new BallSizeChangedEvent { NewScale = _currentScale });
            _debug.Log($"Spike hit! Scale: {_currentScale}");

            if (_currentScale <= _config.MinScale)
            {
                _isAlive = false;
                _eventBus.Publish(new BallDestroyedEvent());
            }
        }

        public void OnBottleCollected(Vector3 position)
        {
            _currentScale = Mathf.Min(_config.MaxScale, _currentScale + _config.SizeChangePerBottle);
            ApplyScale();
            _eventBus.Publish(new BottleCollectedEvent { Position = position });
            _eventBus.Publish(new BallSizeChangedEvent { NewScale = _currentScale });
            _debug.Log($"Bottle collected! Scale: {_currentScale}");
        }

        public void SetColor(Color color)
        {
            _currentColor = color;
            _eventBus.Publish(new BallColorChangedEvent { NewColor = color });

            if (_trail != null)
            {
                var gradient = new Gradient();
                gradient.SetKeys(
                    new[] { new GradientColorKey(color, 0f), new GradientColorKey(color * 0.5f, 1f) },
                    new[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(0f, 1f) }
                );
                _trail.colorGradient = gradient;
            }
        }

        private void ApplyScale()
        {
            if (_transform != null)
                _transform.localScale = Vector3.one * _currentScale;
        }
    }
}
