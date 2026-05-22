using UnityEngine;

[RequireComponent(typeof(UFO))]
public class UFOCollisionHandler : MonoBehaviour
{
    private UFO _ufo;

    private void Awake()
    {
        _ufo = GetComponent<UFO>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out Player player) || other.TryGetComponent(out InvulnerableCircle invulnerableCircle))
        {
            _ufo.Facade.Bounce(other);
        }
    }
}