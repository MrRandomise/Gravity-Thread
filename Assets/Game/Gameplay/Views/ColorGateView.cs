using UnityEngine;

namespace GravityThread.Gameplay.Views
{
    public sealed class ColorGateView : MonoBehaviour
    {
        [SerializeField] private Color _requiredColor = Color.green;

        public Color RequiredColor => _requiredColor;
    }
}
