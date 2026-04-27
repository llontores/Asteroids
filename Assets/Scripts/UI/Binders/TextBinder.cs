using System;
using TMPro;
using UniRx;
using MVVM;

public class TextBinder : IBinder
{
    private readonly TMP_Text _view;
    private readonly IReadOnlyReactiveProperty<string> _property;
    private IDisposable _disposable;

    public TextBinder(TMP_Text view, IReadOnlyReactiveProperty<string> property)
    {
        _view = view;
        _property = property;
    }

    public void Bind()
    {
        if (_view == null || _property == null) return;

        _view.text = _property.Value;

        _disposable = _property.Subscribe(newValue => 
        {
            _view.text = newValue;
        });
    }

    public void Unbind()
    {
        _disposable?.Dispose();
        _disposable = null;
    }
}