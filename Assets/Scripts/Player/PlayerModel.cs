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
    private int _invulnerabilityCoolDown;
    private InvulnerableCircle _invulnerableCircle;
    private SignalBus _signalBus;

    [Inject]
    public void Construct(Player player, SignalBus signalBus)
    {
        _player = player;
        _signalBus = signalBus;
    }

    public void Initialize()
    {
        _player.OnTriggerEntered += StartInvulnerability;
        _invulnerableCircle = _player.InvulnerableEffectCircle;
        _invulnerableCircle.gameObject.SetActive(false);
        _polygonCollider2D = _player.PolygonCollider2D;
        _invulnerabilityDuration = _player.Config.InvulnerabilityDuration;
        _invulnerabilityCoolDown = _player.Config.InvulnerabilityCoolDown;
        _signalBus.Subscribe<RestartButtonPressedSignal>(Reset);
    }

    public void Dispose()
    {
        _player.OnTriggerEntered -= StartInvulnerability;
        _signalBus.Unsubscribe<RestartButtonPressedSignal>(Reset);
        CancelToken();
    }

    private void StartInvulnerability(Collider2D collision)
    {
        if (_polygonCollider2D.enabled)
        {
            TurnOffCollider().Forget();
        }
    }

    private async UniTaskVoid TurnOffCollider()
    {
        CancelToken();
        _cancellationTokenSource = new CancellationTokenSource();
        var token = _cancellationTokenSource.Token;

        try
        {
            _polygonCollider2D.enabled = false;
            _invulnerableCircle.gameObject.SetActive(true);
            _player.ChangeInvelnurabilityStatus(true);

            await UniTask.Delay(_invulnerabilityDuration, cancellationToken: token);

            _player.ChangeInvelnurabilityStatus(false);
            _invulnerableCircle.gameObject.SetActive(false);

            await UniTask.Delay(_invulnerabilityCoolDown, cancellationToken: token);

            _polygonCollider2D.enabled = true;
        }
        catch (OperationCanceledException) { }
    }

    private void Reset()
    {
        CancelToken();
        _polygonCollider2D.enabled = true;
        _invulnerableCircle.gameObject.SetActive(false);
        _player.ChangeInvelnurabilityStatus(false);
    }

    private void CancelToken()
    {
        if (_cancellationTokenSource != null)
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
            _cancellationTokenSource = null;
        }
    }
}