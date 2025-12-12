using DefaultNamespace;
using UnityEngine;

public class UFO : MonoBehaviour, IDestroyable
{
    public void Destroy(DestroyReason reason)
    {
        throw new System.NotImplementedException();
    }
}