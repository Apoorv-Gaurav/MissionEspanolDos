
using UnityEngine;

public class L02_PlayerMovement : MonoBehaviour
{
    public float speed = 5f;

    private Rigidbody2D rb;
    private Vector2 movement;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        // Prevent faster diagonal movement
        movement = movement.normalized;
    }

    void FixedUpdate()
    {
        rb.linearVelocity = movement * speed;
    }
}
