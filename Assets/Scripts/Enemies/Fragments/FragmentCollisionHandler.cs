using UnityEngine;

[RequireComponent(typeof(Fragment))]
public class FragmentCollisionHandler : MonoBehaviour
{
    private Fragment _fragment;

    private void Awake()
    {
        _fragment = GetComponent<Fragment>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out Player player) || other.TryGetComponent(out InvulnerableCircle invulnerableCircle))
        {
            _fragment.Facade.Bounce(other);
        }
    }
}