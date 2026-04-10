using System.Reflection;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    float scaleAmount = 0.2f;

    private Rigidbody2D rb;
    private Vector2 movement;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        
    }

    void Update()
    {
        movement = Vector2.zero;

        if (Keyboard.current.wKey.isPressed)
            movement.y += 1;

        if (Keyboard.current.sKey.isPressed)
            movement.y -= 1;

        if (Keyboard.current.aKey.isPressed)
            movement.x -= 1;

        if (Keyboard.current.dKey.isPressed)
            movement.x += 1;

        movement = movement.normalized;

        if (Gamepad.current != null && movement == Vector2.zero)
        {
            movement = Gamepad.current.leftStick.ReadValue();
        }

        if (Gamepad.current != null && Gamepad.current.buttonSouth.wasPressedThisFrame)
        {
            if (transform.localScale.x < 10) transform.localScale += new Vector3(scaleAmount, scaleAmount, 0);
        }
        if (Gamepad.current != null && Gamepad.current.buttonNorth.wasPressedThisFrame)
        {
            if (transform.localScale.x > 0) transform.localScale -= new Vector3(scaleAmount, scaleAmount, 0);
        }
    }
    

    void FixedUpdate()
    {
        rb.linearVelocity = movement * moveSpeed;
    }
}