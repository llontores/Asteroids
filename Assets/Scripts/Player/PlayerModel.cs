using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

public class PlayerModel : IInitializable, IDisposable
{
    private Player _player;
    private PolygonCollider2D _polygonCollider2D;
    private CancellationTokenSource _cancellationTokenSource;
    private int _invulnerabilityDuration;

    [Inject]
    public void Construct(Player player)
    {
        _player = player;
        _polygonCollider2D = _player.PolygonCollider2D;
        _invulnerabilityDuration = _player.InvulnerabilityDuration;
        _cancellationTokenSource = new CancellationTokenSource();
    }

    public void Initialize()
    {
        _player.OnTriggerEntered += StartInvulnerability;
    }

    public void Dispose()
    {
        _player.OnTriggerEntered -= StartInvulnerability;
        _cancellationTokenSource.Cancel();
        _cancellationTokenSource.Dispose();
    }

    private void StartInvulnerability(Collider2D collision)
    {
        TurnOffCollider().Forget();
    }

    private async UniTaskVoid TurnOffCollider()
    {
        _polygonCollider2D.enabled = false;
        await UniTask.Delay(_invulnerabilityDuration, cancellationToken: _cancellationTokenSource.Token);
        _polygonCollider2D.enabled = true;
    }
}