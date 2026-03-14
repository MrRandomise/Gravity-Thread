using DG.Tweening;
using UnityEngine;

namespace GravityThread.Gameplay.Views
{
    public sealed class BottleView : MonoBehaviour
    {
        [SerializeField] private float _bobAmplitude = 0.15f;
        [SerializeField] private float _bobDuration = 1f;
        [SerializeField] private float _glowIntensity = 1.5f;

        private SpriteRenderer _renderer;

        private void Awake()
        {
            _renderer = GetComponent<SpriteRenderer>();
        }

        private void Start()
        {
            transform.DOLocalMoveY(transform.localPosition.y + _bobAmplitude, _bobDuration)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.InOutSine);

            if (_renderer != null)
            {
                _renderer.material.SetFloat("_GlowIntensity", _glowIntensity);
            }
        }

        private void OnDestroy()
        {
            transform.DOKill();
        }
    }
}
