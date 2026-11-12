using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SocialPlatforms;
using static UnityEngine.GraphicsBuffer;

public class HorseMovement : MonoBehaviour
{
    public float maxSpeed = 12f;
    public float acceleration = 9f;
    public float deceleration = 12f;

    public float maxTurnSpeed = 200f; // Max rotation speed
    public float turnAcceleration = 300f;  // How fast it starts turning

    public float grip = 2f; // How much the horse drifts in a turn (1 is no drift)

    private Rigidbody2D rb;

    private float x;
    private float y;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Reading input
        x = Input.GetAxis("Horizontal");
        y = Input.GetAxis("Vertical");
    }

    void FixedUpdate()
    {
        Move();
    }

    void Move()
    {
        // Casting these to a vector2 drops the z axis so the math can leave it out later
        Vector2 upVector = transform.up;
        Vector2 rightVector = transform.right;

        // Get target speed and rate of change
        float targetSpeed = y * maxSpeed;
        float rate = Mathf.Abs(y) > 0 ? acceleration : deceleration;

        // Get target rotating speed and rate of roation
        float targetRotation = y * maxSpeed;
        float rotationRate = Mathf.Abs(y) > 0 ? acceleration : deceleration;

        // Calculate a new speed
        float currentSpeed = Vector2.Dot(rb.linearVelocity, upVector);
        float newSpeed = Mathf.MoveTowards(
            currentSpeed,
            targetSpeed,
            rate * Time.fixedDeltaTime
        );

        // Speed to retain during a turn
        float retainedSpeed = Vector2.Dot(rb.linearVelocity * grip, rightVector);

        // Do the same for rotation as we do for speed (this is to add a cap on how fast players can rotate and to make it so perks can adjust it)
        float targetTurnSpeed = -x * maxTurnSpeed;
        float turnRate = turnAcceleration;

        float currentTurnSpeed = rb.angularVelocity;
        float newTurnSpeed = Mathf.MoveTowards(
            currentTurnSpeed,
            targetTurnSpeed,
            turnRate * Time.fixedDeltaTime
        );

        // Apply changes
        rb.linearVelocity = (upVector * newSpeed) + (rightVector * retainedSpeed * 0.2f);
        rb.angularVelocity = newTurnSpeed;
    }
}