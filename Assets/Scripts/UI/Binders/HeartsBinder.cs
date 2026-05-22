using System;
using UniRx;
using MVVM;

public class HeartsBinder : IBinder
{
    private readonly HeartsView _view;
    private readonly IReadOnlyReactiveProperty<HealthData> _property;
    private IDisposable _disposable;

    public HeartsBinder(HeartsView view, IReadOnlyReactiveProperty<HealthData> property)
    {
        _view = view;
        _property = property;
    }

    public void Bind()
    {
        if (_view == null || _property == null) return;

        _view.UpdateView(_property.Value);
        
        _disposable = _property.Subscribe(newValue => 
        {
            _view.UpdateView(newValue);
        });
    }

    public void Unbind()
    {
        _disposable?.Dispose();
        _disposable = null;
    }
}