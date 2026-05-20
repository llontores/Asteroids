using UnityEngine;

public interface ITarget : IDestroyable
{
    Vector2 Position { get; }
    float ColliderRadius { get; }
}