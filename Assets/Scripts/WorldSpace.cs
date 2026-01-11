using UnityEngine;

public class WorldSpace : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out IDestroyable destroyable))
        {
            destroyable.Destroy(DestroyReason.World);
        }
    }
}