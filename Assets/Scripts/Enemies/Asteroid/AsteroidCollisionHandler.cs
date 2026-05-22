using UnityEngine;

[RequireComponent(typeof(Asteroid))]
public class AsteroidCollisionHandler : MonoBehaviour
{
    private Asteroid _asteroid;

    private void Awake()
    {
        _asteroid = GetComponent<Asteroid>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out Player player) || other.TryGetComponent(out InvulnerableCircle invulnerableCircle))
        {
            _asteroid.Facade.Bounce(other);
        }
    }
}