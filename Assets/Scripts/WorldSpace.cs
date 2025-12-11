using Signals;
using UnityEngine;
using Zenject;

namespace DefaultNamespace
{
    public class WorldSpace : MonoBehaviour
    {
        [SerializeField] private HazardSpawner _hazardSpawner;
        
        private PlayerShooter _playerShooter;
        private SignalBus _signalBus;

        [Inject]
        public void Construct(PlayerShooter playerShooter, SignalBus signalBus)
        {
            _playerShooter = playerShooter;
            _signalBus = signalBus;
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.TryGetComponent(out Bullet bullet))
            {
                _playerShooter.BulletPool.ReturnObject(bullet);
            }

            if (other.TryGetComponent(out Asteroid asteroid))
            {
                asteroid.Die();
            }

            if (other.TryGetComponent(out UFO ufo))
            {
                _hazardSpawner.ReturnUFOToPool(ufo);
            }
        }
    }
}