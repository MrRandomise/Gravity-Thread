using GravityThread.Configs;
using GravityThread.Core.Events;
using GravityThread.Core.Interfaces;
using GravityThread.Services.Input;
using UnityEngine;

namespace GravityThread.Gameplay
{
    public sealed class ThreadSystem : IGameTickable
    {
        private readonly ThreadConfig _config;
        private readonly IInputService _input;
        private readonly EventBus _eventBus;

        private float _currentEnergy;
        private float _regenCooldown;
        private bool _isActive;

        public float CurrentEnergy => _currentEnergy;
        public float NormalizedEnergy => _currentEnergy / _config.MaxEnergy;
        public bool IsActive => _isActive;
        public float TotalEnergyUsed { get; private set; }

        public ThreadSystem(ThreadConfig config, IInputService input, EventBus eventBus)
        {
            _config = config;
            _input = input;
            _eventBus = eventBus;
            _currentEnergy = config.MaxEnergy;
        }

        public void Reset()
        {
            _currentEnergy = _config.MaxEnergy;
            TotalEnergyUsed = 0f;
            _regenCooldown = 0f;
            PublishEnergyUpdate();
        }

        public void GameTick(float deltaTime)
        {
            if (_input.IsTouching && _currentEnergy > 0f)
            {
                _isActive = true;
                float drain = _config.DrainRate * deltaTime;
                TotalEnergyUsed += drain;
                _currentEnergy = Mathf.Max(0f, _currentEnergy - drain);
                _regenCooldown = _config.RegenDelay;
            }
            else
            {
                _isActive = false;
                _regenCooldown -= deltaTime;
                if (_regenCooldown <= 0f)
                {
                    _currentEnergy = Mathf.Min(_config.MaxEnergy, _currentEnergy + _config.RegenRate * deltaTime);
                }
            }

            PublishEnergyUpdate();
        }

        public Vector2 CalculateForce(Vector2 ballPosition)
        {
            if (!_isActive) return Vector2.zero;

            Vector2 touchPos = _input.TouchWorldPosition;
            Vector2 direction = touchPos - ballPosition;
            float distance = direction.magnitude;

            if (distance > _config.MaxDistance) return Vector2.zero;

            float distanceFactor = 1f - (_config.DistanceFalloff * (distance / _config.MaxDistance));
            return direction.normalized * (_config.PullForce * distanceFactor);
        }

        private void PublishEnergyUpdate()
        {
            _eventBus.Publish(new ThreadEnergyChangedEvent
            {
                Current = _currentEnergy,
                Max = _config.MaxEnergy
            });
        }
    }
}
