using UnityEngine;
using UnityEngine.InputSystem;

public class Move : MonoBehaviour
{
    private PlayerControls controls;
    private Vector2 moveInput;
    public float speed = 5f;

    private void Awake()
    {
        controls = new PlayerControls();

        controls.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled += ctx => moveInput = Vector2.zero;
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }

    private void Update()
    {
        Vector3 movement = new Vector3(moveInput.x, moveInput.y, 0f);
        transform.position += movement * speed * Time.deltaTime;
    }
}
