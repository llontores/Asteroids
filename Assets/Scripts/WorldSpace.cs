using Signals;
using UnityEngine;
using Zenject;

namespace DefaultNamespace
{
    public class WorldSpace : MonoBehaviour
    {
        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.TryGetComponent(out Bullet bullet))
            {
                bullet.Destroy();
            }

            if (other.TryGetComponent(out Asteroid asteroid))
            {
                asteroid.Die();
            }

            if (other.TryGetComponent(out UFO ufo))
            {

            }
        }
    }
}