using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class CustomRaycaster
{
    private readonly TargetsRegistry _registry;

    [Inject]
    public CustomRaycaster(TargetsRegistry registry)
    {
        _registry = registry;
    }

    public RaycastResult RaycastAll(Vector2 origin, Vector2 direction, float maxDistance)
    {
        var result = new RaycastResult
        {
            HasHits = false,
            HitTargets = new List<ITarget>(),
            FurthestPoint = origin + direction * maxDistance
        };

        float maxHitDistance = -1f;

        foreach (var target in _registry.ActiveTargets)
        {
            if (target == null) continue;

            Vector2 center = target.Position;
            float radius = target.ColliderRadius;

            Vector2 toCenter = center - origin;
            float projectionLength = Vector2.Dot(toCenter, direction);

            if (projectionLength < 0 || projectionLength > maxDistance)
                continue;

            Vector2 closestPointOnRay = origin + direction * projectionLength;
            float distanceToRay = Vector2.Distance(center, closestPointOnRay);

            if (distanceToRay <= radius)
            {
                result.HasHits = true;
                result.HitTargets.Add(target);

                if (projectionLength > maxHitDistance)
                {
                    maxHitDistance = projectionLength;
                    result.FurthestPoint = center;
                }
            }
        }

        return result;
    }
}