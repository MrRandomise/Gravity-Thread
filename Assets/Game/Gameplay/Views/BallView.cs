using UnityEngine;
using Zenject;

namespace GravityThread.Gameplay.Views
{
    [RequireComponent(typeof(Rigidbody2D), typeof(CircleCollider2D), typeof(TrailRenderer))]
    public sealed class BallView : MonoBehaviour
    {
        private BallController _ball;
        private Rigidbody2D _rb;
        private TrailRenderer _trail;

        [Inject]
        public void Construct(BallController ball)
        {
            _ball = ball;
        }

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _trail = GetComponent<TrailRenderer>();
        }

        private void Start()
        {
            _ball.Initialize(_rb, _trail);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (!_ball.IsAlive) return;

            float impactForce = collision.relativeVelocity.magnitude;

            if (collision.gameObject.CompareTag("Wall"))
            {
                _ball.OnWallCollision(impactForce);
            }
            else if (collision.gameObject.CompareTag("Spike"))
            {
                _ball.OnSpikeHit(collision.contacts[0].point);
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!_ball.IsAlive) return;

            if (other.CompareTag("Bottle"))
            {
                _ball.OnBottleCollected(other.transform.position);
                Destroy(other.gameObject);
            }
            else if (other.CompareTag("ColorSource"))
            {
                var colorSource = other.GetComponent<ColorSourceView>();
                if (colorSource != null)
                    _ball.SetColor(colorSource.GateColor);
            }
            else if (other.CompareTag("ColorGate"))
            {
                var gate = other.GetComponent<ColorGateView>();
                if (gate != null && _ball.CurrentColor == gate.RequiredColor)
                {
                    Destroy(other.gameObject);
                }
            }
            else if (other.CompareTag("Goal"))
            {
                var levelManager = FindObjectOfType<LevelContext>();
                if (levelManager != null)
                    levelManager.OnGoalReached();
            }
        }
    }
}
