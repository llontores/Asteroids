using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    [SerializeField] private int _reward;
    
    public int Reward => _reward;
    public DestroyReason Reason { get; private set; }

    protected void SetDestroyReason(DestroyReason reason)
    {
        Reason = reason;
    }
}