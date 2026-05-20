using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

public class PlayerFacade : IInitializable, IDisposable
{
    private Player _player;
    private PolygonCollider2D _invulnerabilityCollider;
    private CancellationTokenSource _cancellationTokenSource;
    private int _invulnerabilityDuration;
    private int _invulnerabilityCoolDown;
    private InvulnerableCircle _invulnerableCircle;
    private SignalBus _signalBus;
    private PlayerConfig _config;
    private PlayerReferences _playerReferences;
    private int _currentHealth;
    private int _maxHealth;
    public bool IsInvulnerable { get; private set; }
    public int MaxHealth => _maxHealth;
    public int CurrentHealth => _currentHealth;
    
    [Inject]
    public void Construct( SignalBus signalBus, PlayerConfig config, PlayerReferences playerReferences, Player player)
    {
        _player = player;
        _playerReferences =  playerReferences;
        _config =  config;
        _signalBus = signalBus;
    }

    public void Initialize()
    {
        _player.OnTriggerEntered += NandleCollision;
        _invulnerableCircle = _playerReferences.InvulnerableEffectCircle;
        _invulnerableCircle.gameObject.SetActive(false);
        _invulnerabilityCollider = _playerReferences.PolygonCollider2D;
        _invulnerabilityDuration = _config.InvulnerabilityDuration;
        _invulnerabilityCoolDown = _config.InvulnerabilityCoolDown;
        _maxHealth = _config.MaxHealth;
        _currentHealth = _maxHealth;
        _signalBus.Subscribe<RestartButtonPressedSignal>(Reset);
    }

    public void Dispose()
    {
        _player.OnTriggerEntered -= NandleCollision;
        _signalBus.Unsubscribe<RestartButtonPressedSignal>(Reset);
        
        CancelToken();
    }

    private void NandleCollision(Collider2D collision)
    {
        if (IsInvulnerable)
            return;

        if (collision.TryGetComponent<Bullet>(out Bullet _))
            return;
        
        TakeDamage();

        if (_invulnerabilityCollider.enabled)
        {
            TurnOffCollider().Forget();
        }
    }

    private void TakeDamage()
    {
        _currentHealth = Mathf.Clamp(_currentHealth - 1, 0, _config.MaxHealth);

        if (_currentHealth <= 0)
            _signalBus.Fire(new PlayerDeadSignal());
    }

    private async UniTaskVoid TurnOffCollider()
    {
        CancelToken();
        _cancellationTokenSource = new CancellationTokenSource();
        var token = _cancellationTokenSource.Token;

        try
        {
            _invulnerabilityCollider.enabled = false;
            _invulnerableCircle.gameObject.SetActive(true);
            SetInvulnerable(true);

            await UniTask.Delay(_invulnerabilityDuration, cancellationToken: token);

            SetInvulnerable(false);
            _invulnerableCircle.gameObject.SetActive(false);

            await UniTask.Delay(_invulnerabilityCoolDown, cancellationToken: token);

            _invulnerabilityCollider.enabled = true;
        }
        catch (OperationCanceledException) { }
    }

    private void Reset()
    {
        ResetHealth();
        CancelToken();
        _invulnerabilityCollider.enabled = true;
        _invulnerableCircle.gameObject.SetActive(false);
        SetInvulnerable(false);
    }
    
    private void SetInvulnerable(bool status)
    {
        IsInvulnerable = status;
    }
    
    private void ResetHealth()
    {
        _currentHealth = _config.MaxHealth;
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