using System.Collections.Generic;
using UnityEngine;

public struct RaycastResult
{
    public bool HasHits;
    public Vector2 FurthestPoint;
    public List<ITarget> HitTargets;
}