using UnityEngine;
using Zenject;

namespace DefaultNamespace
{
    public class WorldSpace : MonoBehaviour
    {
        private PlayerShooter _playerShooter;

        [Inject]
        public void Construct(PlayerShooter playerShooter)
        {
            _playerShooter = playerShooter;
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.TryGetComponent(out Bullet bullet))
            {
                _playerShooter.BulletPool.ReturnObject(bullet);
            }
        }
    }
}