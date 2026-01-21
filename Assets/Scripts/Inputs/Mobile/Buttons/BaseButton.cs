using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ActionButton : Button
{
    public event UnityAction Pressed;
    public event UnityAction Released;

    [SerializeField] private bool _hold;

    private bool _holding;

    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);

        if (_hold)
        {
            _holding = true;
            return;
        }

        Pressed?.Invoke();
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerUp(eventData);

        if (_hold)
        {
            _holding = false;
        }

        Released?.Invoke();
    }

    private void Update()
    {
        if (_holding)
        {
            Pressed?.Invoke();
        }
    }
}