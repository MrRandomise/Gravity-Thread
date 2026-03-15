using System.Collections.Generic;
using UnityEngine;

namespace GravityThread.Configs
{
    [CreateAssetMenu(fileName = "AudioConfig", menuName = "GravityThread/AudioConfig")]
    public sealed class AudioConfig : ScriptableObject
    {
        [Header("Background Music")]
        [SerializeField] private List<AudioClip> _backgroundMusic = new List<AudioClip>();
        [SerializeField, Range(0f, 1f)] private float _musicVolume = 0.5f;
        [SerializeField] private bool _loop = true;

        public List<AudioClip> BackgroundMusic => _backgroundMusic;
        public float MusicVolume => _musicVolume;
        public bool Loop => _loop;
    }
}