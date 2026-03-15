using System.Collections.Generic;
using UnityEngine;
using GravityThread.Core.Interfaces;

namespace GravityThread.Core
{
    /// <summary>
    /// Single MonoBehaviour entry point that dispatches Unity lifecycle callbacks
    /// to all registered tickable systems. This ensures minimal MonoBehaviour usage.
    /// </summary>
    public sealed class GameLifecycleRunner : MonoBehaviour
    {
        private readonly List<IGameTickable> _tickables = new List<IGameTickable>();
        private readonly List<IGameFixedTickable> _fixedTickables = new List<IGameFixedTickable>();
        private readonly List<IGameLateTickable> _lateTickables = new List<IGameLateTickable>();

        private bool _isPaused;

        public bool IsPaused
        {
            get => _isPaused;
            set => _isPaused = value;
        }

        public void Register(object system)
        {
            if (system is IGameTickable tickable)
                _tickables.Add(tickable);
            if (system is IGameFixedTickable fixedTickable)
                _fixedTickables.Add(fixedTickable);
            if (system is IGameLateTickable lateTickable)
                _lateTickables.Add(lateTickable);
        }

        public void Unregister(object system)
        {
            if (system is IGameTickable tickable)
                _tickables.Remove(tickable);
            if (system is IGameFixedTickable fixedTickable)
                _fixedTickables.Remove(fixedTickable);
            if (system is IGameLateTickable lateTickable)
                _lateTickables.Remove(lateTickable);
        }

        private void Update()
        {
            if (_isPaused) return;
            float dt = Time.deltaTime;
            for (int i = 0, count = _tickables.Count; i < count; i++)
                _tickables[i].GameTick(dt);
        }

        private void FixedUpdate()
        {
            if (_isPaused) return;
            float fdt = Time.fixedDeltaTime;
            for (int i = 0, count = _fixedTickables.Count; i < count; i++)
                _fixedTickables[i].GameFixedTick(fdt);
        }

        private void LateUpdate()
        {
            if (_isPaused) return;
            float dt = Time.deltaTime;
            for (int i = 0, count = _lateTickables.Count; i < count; i++)
                _lateTickables[i].GameLateTick(dt);
        }
    }
}
