using UnityEngine;
using UnityEngine.InputSystem;

public class HorseMovement : MonoBehaviour
{
    public float maxSpeed = 6f;
    public float acceleration = 4f;
    public float deceleration = 6f;

    private Rigidbody2D rb;
    private Vector2 input;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        float x = 0f;
        float y = 0f;

        if (Keyboard.current.aKey.isPressed) x = -1f;
        if (Keyboard.current.dKey.isPressed) x = 1f;
        if (Keyboard.current.wKey.isPressed) y = 1f;
        if (Keyboard.current.sKey.isPressed) y = -1f;

        input = new Vector2(x, y).normalized;
    }

    void FixedUpdate()
    {
        Vector2 target = input * maxSpeed;

        float rate = input.magnitude > 0
            ? acceleration : deceleration;

        rb.linearVelocity = Vector2.MoveTowards(
            rb.linearVelocity,
            target,
            rate * Time.fixedDeltaTime
        );
    }
}
