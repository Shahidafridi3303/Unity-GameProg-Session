using UnityEngine;

public enum MovementDirection
{
    Horizontal,
    Vertical,
    Diagonal
}

public enum InitialMovement
{
    Forward,
    Backward
}

public class MovingPlatform : MonoBehaviour
{
    [Header("Movement Settings")]
    public MovementDirection movementDirection = MovementDirection.Horizontal;
    public float speed = 2f;         // Speed of movement
    public float minDistance = 0f;   // Minimum movement limit
    public float maxDistance = 5f;   // Maximum movement limit

    [Header("Initial Movement Settings")]
    public InitialMovement initialMovement = InitialMovement.Forward;

    private Vector3 startPos;
    private int direction; // 1 = forward, -1 = backward

    [HideInInspector] public bool canPerform = true;

    void Start()
    {
        startPos = transform.position;
        direction = (initialMovement == InitialMovement.Forward) ? 1 : -1;
    }

    void FixedUpdate()
    {
        if (canPerform)
        {
            MovePlatform();
        }
    }

    void MovePlatform()
    {
        Vector3 moveVector = Vector3.zero;

        switch (movementDirection)
        {
            case MovementDirection.Horizontal:
                moveVector = Vector3.right;
                break;
            case MovementDirection.Vertical:
                moveVector = Vector3.up;
                break;
            case MovementDirection.Diagonal:
                moveVector = new Vector3(1, 1, 0).normalized; // Moves diagonally
                break;
        }

        transform.position += moveVector * speed * direction * Time.deltaTime;

        // Reverse direction when reaching limits
        float distanceMoved = Vector3.Distance(startPos, transform.position);
        if (distanceMoved >= maxDistance || distanceMoved <= minDistance)
        {
            direction *= -1;
        }
    }
}