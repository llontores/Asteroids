using Signals;
using UnityEngine;
using Zenject;


    public class WorldSpace : MonoBehaviour
    {
        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.TryGetComponent(out IDestroyable destroyable))
            {
                destroyable.Destroy(DestroyReason.World);
            }
        }
    }
