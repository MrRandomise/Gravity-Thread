using UnityEngine;

namespace GravityThread.Gameplay.Views
{
    public sealed class ColorSourceView : MonoBehaviour
    {
        [SerializeField] private Color _gateColor = Color.green;

        public Color GateColor => _gateColor;
    }
}
