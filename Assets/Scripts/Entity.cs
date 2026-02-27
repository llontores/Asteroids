using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    public int Reward => _reward;

    protected int _reward;
    public DestroyReason Reason { get; private set; }
    
    protected void SetDestroyReason(DestroyReason reason)
    {
        Reason = reason;
    }
}