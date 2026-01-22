using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HoldingButton : Button
{
    public event UnityAction ButtonPressed;
    public event UnityAction ButtonReleased;

    private bool _isHolding;

    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        _isHolding = true;
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerUp(eventData);
        _isHolding = false;
        ButtonReleased?.Invoke();
    }

    private void Update()
    {
        if (_isHolding)
            ButtonPressed?.Invoke();
    }
}