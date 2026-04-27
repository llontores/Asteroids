using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class Joystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [SerializeField] private RectTransform _background;
    [SerializeField] private RectTransform _handle;
    [SerializeField] private float _deadzone = 0.2f; 
    
    public event Action<float> OnAccelerateChanged; 
    public event Action<int> OnTurnChanged;

    private Vector2 _inputVector;
    private bool _isPressed;

    public void OnPointerDown(PointerEventData eventData)
    {
        _isPressed = true;
        _handle.position = _background.position;
        OnDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(_background, eventData.position, eventData.pressEventCamera, out Vector2 localPoint))
        {
            float radius = _background.rect.width / 2f;
            _inputVector = localPoint / radius;

            if (_inputVector.magnitude > 1.0f)
                _inputVector = _inputVector.normalized;

            Vector3 offset = _inputVector * (radius * _background.lossyScale.x);
            _handle.position = _background.position + offset;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _isPressed = false;
        _inputVector = Vector2.zero;
        _handle.position = _background.position;
        OnAccelerateChanged?.Invoke(0f); 
    }

    private void Update()
    {
        if (!_isPressed) return;
        
        float accelerationValue = Mathf.Clamp01(_inputVector.y); 
        
        if (accelerationValue < _deadzone) accelerationValue = 0f;

        OnAccelerateChanged?.Invoke(accelerationValue);
        
        int currentTurn = 0;
        if (_inputVector.x < -_deadzone) currentTurn = 1;
        else if (_inputVector.x > _deadzone) currentTurn = -1;

        OnTurnChanged?.Invoke(currentTurn);
    }
}