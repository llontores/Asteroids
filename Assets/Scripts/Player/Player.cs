using UnityEngine;
using UnityEngine.Events;

public class Player : MonoBehaviour
{
    public Transform Transform => transform;
    public event UnityAction<Collider2D> OnTriggerEntered;
  
    private void OnTriggerEnter2D(Collider2D collision)
    {
        OnTriggerEntered?.Invoke(collision);
    }
}