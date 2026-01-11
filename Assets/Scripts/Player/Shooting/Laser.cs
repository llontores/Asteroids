using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Signals;
using UnityEngine;
using Zenject;

public class Laser : MonoBehaviour
{
    [Header("Settings")]
    public float maxDistance = 50f;
    public LayerMask ignoreLayers; // Выбери здесь слой Игрока, чтобы луч не попадал в себя

    private LineRenderer _lineRenderer;

    void Start()
    {
        // Настраиваем LineRenderer программно, чтобы точно работало
        _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.positionCount = 2;
        _lineRenderer.startWidth = 0.1f;
        _lineRenderer.endWidth = 0.1f;
    }

    void Update()
    {
        // Проверяем нажатие правой кнопки мыши
        if (Input.GetMouseButton(1))
        {
            _lineRenderer.enabled = true;
            UpdateLaser();
        }
        else
        {
            _lineRenderer.enabled = false;
        }
    }

    void UpdateLaser()
    {
        // 1. Определяем точку старта и направление
        Vector2 origin = transform.position;
        Vector2 direction = transform.up; // В 2D обычно 'up' — это направление ствола

        // 2. Пускаем физический луч (Raycast)
        // Он возвращает информацию о том, во что врезался
        RaycastHit2D hit = Physics2D.Raycast(origin, direction, maxDistance, ~ignoreLayers);

        Vector2 endPoint;

        // 3. Проверяем: врезался ли луч во что-то?
        if (hit.collider != null)
        {
            // Если врезался — берем точку контакта
            endPoint = hit.point;
            
            // Здесь можно добавить искры в месте попадания
            Debug.Log($"Попал в: {hit.collider.name}");
        }
        else
        {
            // Если ничего не встретил — летит на максимальное расстояние
            endPoint = origin + (direction * maxDistance);
        }

        // 4. Отрисовка
        _lineRenderer.SetPosition(0, new Vector3(origin.x,origin.y, -0.1f));   // Начало линии
        _lineRenderer.SetPosition(1, new Vector3(endPoint.x, endPoint.y, -0.1f)); // Конец линии
    }
}
