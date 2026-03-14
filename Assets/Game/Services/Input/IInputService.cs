using UnityEngine;

namespace GravityThread.Services.Input
{
    public interface IInputService
    {
        bool IsTouching { get; }
        Vector2 TouchWorldPosition { get; }
    }
}
