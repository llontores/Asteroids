using UnityEngine;

public class SpaceShipPhysics : MonoBehaviour
{
    // === MOVEMENT STATE ===
    public Vector2 Velocity;
    private Vector2 acceleration;

    // === MOVEMENT SETTINGS ===
    public float Thrust = 6f;       // ускорение при газе
    public float Drag = 1.5f;       // торможение
    public float MaxSpeed = 10f;    // максимальная скорость

    // === ROTATION SETTINGS ===
    public float TurnSpeed = 180f; // градусы в секунду

    // === WORLD LIMITS ===
    public float MinX = -8f;
    public float MaxX = 8f;
    public float MinY = -4f;
    public float MaxY = 4f;

    void Update()
    {
        HandleInput();
        UpdatePhysics(Time.deltaTime);
        MoveShip(Time.deltaTime);
        HandleBounce();
    }

    // ===========================
    // 1. INPUT → ACCELERATION
    // ===========================
    void HandleInput()
    {
        acceleration = Vector2.zero;

        // ВРАЩЕНИЕ
        float turn = 0f;
        if (Input.GetKey(KeyCode.A)) turn = +1f;
        if (Input.GetKey(KeyCode.D)) turn = -1f;

        transform.Rotate(0, 0, turn * TurnSpeed * Time.deltaTime); //сделаю в мувмент хендлере

        // ГАЗ (ускорение вперёд)
        if (Input.GetKey(KeyCode.W))
        {
            Vector2 forward = transform.up;  // в Unity "вверх" объекта — это его forward в 2D
            acceleration += forward * Thrust;
        }
    }

    // ===========================
    // 2. PHYSICS UPDATE
    // ===========================
    void UpdatePhysics(float dt)
    {
        // acceleration → velocity
        Velocity += acceleration * dt;

        // drag
        Velocity *= 1f / (1f + Drag * dt);

        // limit speed
        if (Velocity.sqrMagnitude > MaxSpeed * MaxSpeed)
            Velocity = Velocity.normalized * MaxSpeed;
    }

    // ===========================
    // 3. APPLY MOVEMENT
    // ===========================
    void MoveShip(float dt)
    {
        transform.position += (Vector3)(Velocity * dt);
    }

    // ===========================
    // 4. BOUNCE FROM WORLD BORDERS
    // ===========================
    void HandleBounce()
    {
        Vector3 pos = transform.position;

        bool bouncedX = false;
        bool bouncedY = false;

        if (pos.x < MinX) { pos.x = MinX; bouncedX = true; }
        if (pos.x > MaxX) { pos.x = MaxX; bouncedX = true; }
        if (pos.y < MinY) { pos.y = MinY; bouncedY = true; }
        if (pos.y > MaxY) { pos.y = MaxY; bouncedY = true; }

        // отскок — инвертируем нужную компоненту скорости
        if (bouncedX) Velocity.x = -Velocity.x * 0.9f; // 0.9 – потеря энергии
        if (bouncedY) Velocity.y = -Velocity.y * 0.9f;

        transform.position = pos;
    }
}
