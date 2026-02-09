
    using System;
    using UnityEngine;
    using Zenject;

    public class LaserView : MonoBehaviour
    {
        [SerializeField] private ParticleSystem _endPointEffect;

        private LaserShooter _shooter;
        
        [Inject]
        public void Construct(LaserShooter shooter)
        {
            _shooter = shooter;
            _shooter.LaserEndPointUpdated += ShowLaserEffect;
            _shooter.LaserTurnedOff += HideLaserEffect;
        }

        private void ShowLaserEffect(Vector3 position)
        {
            _endPointEffect.gameObject.SetActive(true);
            _endPointEffect.transform.position = position;
        }

        private void HideLaserEffect()
        {
            _endPointEffect.gameObject.SetActive(false);
        }

        private void OnDisable()
        {
            _shooter.LaserEndPointUpdated -= ShowLaserEffect;
            _shooter.LaserTurnedOff -= HideLaserEffect;
        }
    }
