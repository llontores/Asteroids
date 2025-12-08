using UnityEngine;
using Zenject;

public class ScreenWrapper : ITickable
{
    private Camera _mainCamera;
    private Vector2 _screenBounds;
    private float _objectWidth;
    private float _objectHeight;
    private Renderer _renderer;
    private Transform _transform;

    [Inject]
    public void Construct(Player player)
    {
        _transform = player.transform;
        _mainCamera = Camera.main;
        _screenBounds = _mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, _mainCamera.transform.position.z));
        _renderer = player.Renderer;
        _objectWidth = _renderer.bounds.extents.x;
        _objectHeight = _renderer.bounds.extents.y;
    }

    public void Tick()
    {
        Vector3 viewPos = _transform.position;
        
        if (viewPos.x > _screenBounds.x + _objectWidth)
        {
            viewPos.x = -_screenBounds.x - _objectWidth;
        }
        else if (viewPos.x < -_screenBounds.x - _objectWidth)
        {
            viewPos.x = _screenBounds.x + _objectWidth;
        }
        
        if (viewPos.y > _screenBounds.y + _objectHeight)
        {
            viewPos.y = -_screenBounds.y - _objectHeight;
        }
        else if (viewPos.y < -_screenBounds.y - _objectHeight)
        {
            viewPos.y = _screenBounds.y + _objectHeight;
        }

        _transform.position = viewPos;
    }
}