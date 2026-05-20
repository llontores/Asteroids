using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ClickButton : Button
{
    public event UnityAction ButtonPressed;

    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        ButtonPressed?.Invoke();
    }
}