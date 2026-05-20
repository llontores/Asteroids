using System.Collections.Generic;

public class TargetsRegistry
{
    private readonly List<ITarget> _activeTargets = new();

    public IReadOnlyList<ITarget> ActiveTargets => _activeTargets;

    public void Register(ITarget target)
    {
        if (!_activeTargets.Contains(target))
            _activeTargets.Add(target);
    }

    public void Unregister(ITarget target)
    {
        _activeTargets.Remove(target);
    }
}