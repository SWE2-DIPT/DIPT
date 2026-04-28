using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float scaleAmount = 0.2f;
    public float rotationAmount = 15f;
    public float shoulderJumpAmount = 0.05f;
    public float shoulderJumpCooldown = 0.15f;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Vector2 movement;
    private Vector2 dpadMove;

    private float nextShoulderJumpTime = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        movement = Vector2.zero;
        dpadMove = Vector2.zero;

        float leftTrigger = 0f;
        float rightTrigger = 0f;

        if (Keyboard.current != null)
        {
            /*if (Keyboard.current.wKey.isPressed)
            {
                movement.y += 1f;
            }

            if (Keyboard.current.sKey.isPressed)
            {
                movement.y -= 1f;
            }

            if (Keyboard.current.aKey.isPressed)
            {
                movement.x -= 1f;
            }

            if (Keyboard.current.dKey.isPressed)
            {
                movement.x += 1f;
            }*/
        }

        movement = movement.normalized;

        if (Gamepad.current != null)
        {
            if (movement == Vector2.zero)
            {
                movement = Gamepad.current.leftStick.ReadValue();
            }

            if (Gamepad.current.buttonSouth.wasPressedThisFrame)
            {
                if (transform.localScale.x < 10f)
                {
                    transform.localScale += new Vector3(scaleAmount, scaleAmount, 0f);
                }
            }

            if (Gamepad.current.buttonNorth.wasPressedThisFrame)
            {
                if (transform.localScale.x > scaleAmount)
                {
                    transform.localScale -= new Vector3(scaleAmount, scaleAmount, 0f);
                }
            }

            if (Gamepad.current.buttonEast.wasPressedThisFrame)
            {
                transform.Rotate(0f, 0f, -rotationAmount);
            }

            if (Gamepad.current.buttonWest.wasPressedThisFrame)
            {
                transform.Rotate(0f, 0f, rotationAmount);
            }

            if (Time.time >= nextShoulderJumpTime)
            {
                if (Gamepad.current.leftShoulder.wasPressedThisFrame)
                {
                    transform.position += new Vector3(0f, -shoulderJumpAmount, 0f);
                    nextShoulderJumpTime = Time.time + shoulderJumpCooldown;
                }

                if (Gamepad.current.rightShoulder.wasPressedThisFrame)
                {
                    transform.position += new Vector3(0f, shoulderJumpAmount, 0f);
                    nextShoulderJumpTime = Time.time + shoulderJumpCooldown;
                }
            }

            if (Gamepad.current.dpad.up.isPressed)
            {
                dpadMove.y += 1f;
            }

            if (Gamepad.current.dpad.down.isPressed)
            {
                dpadMove.y -= 1f;
            }

            if (Gamepad.current.dpad.left.isPressed)
            {
                dpadMove.x -= 1f;
            }

            if (Gamepad.current.dpad.right.isPressed)
            {
                dpadMove.x += 1f;
            }

            leftTrigger = Gamepad.current.leftTrigger.ReadValue();
            rightTrigger = Gamepad.current.rightTrigger.ReadValue();
        }

        if (dpadMove != Vector2.zero)
        {
            movement = dpadMove.normalized;
        }

        if (spriteRenderer != null)
        {
            Color triggerColor = Color.white;
            triggerColor = Color.Lerp(triggerColor, Color.red, leftTrigger);
            triggerColor = Color.Lerp(triggerColor, Color.blue, rightTrigger);
            spriteRenderer.color = triggerColor;
        }
    }

    void FixedUpdate()
    {
        if (rb != null)
        {
            rb.linearVelocity = movement * moveSpeed;
        }
    }
}