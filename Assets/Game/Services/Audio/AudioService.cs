using GravityThread.Configs;
using GravityThread.Core.Events;
using UnityEngine;

namespace GravityThread.Services.Audio
{
    /// <summary>
    /// Manages background music playback. Reacts to pause/resume via EventBus.
    /// </summary>
    public sealed class AudioService
    {
        private readonly AudioSource _source;
        private readonly AudioConfig _config;
        private readonly EventBus _eventBus;

        public AudioService(AudioConfig config, EventBus eventBus)
        {
            _config = config;
            _eventBus = eventBus;

            // Create a persistent AudioSource
            var go = new GameObject("[AudioService]");
            Object.DontDestroyOnLoad(go);
            _source = go.AddComponent<AudioSource>();

            var bgMusic = _config.BackgroundMusic.Count > 0 ? Random.Range(0, _config.BackgroundMusic.Count) : -1;

            _source.clip = bgMusic >= 0 ? _config.BackgroundMusic[bgMusic] : null;
            _source.volume = _config.MusicVolume;
            _source.loop = _config.Loop;
            _source.playOnAwake = false;

            _eventBus.Subscribe<GamePausedEvent>(OnGamePaused);
        }

        public void StartMusic()
        {
            if (_source.clip != null && !_source.isPlaying)
                _source.Play();
        }

        public void StopMusic()
        {
            _source.Stop();
        }

        public void SetVolume(float volume)
        {
            _source.volume = Mathf.Clamp01(volume);
        }

        private void OnGamePaused(GamePausedEvent e)
        {
            if (e.IsPaused)
                _source.Pause();
            else
                _source.UnPause();
        }

        public void Dispose()
        {
            _eventBus.Unsubscribe<GamePausedEvent>(OnGamePaused);
            if (_source != null)
                Object.Destroy(_source.gameObject);
        }
    }
}