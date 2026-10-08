using UnityEngine;
using UnityEngine.InputSystem;

public class Ball : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private string actionName;

    [Header("References")]
    [SerializeField] private Rigidbody2D rb;

    private Vector3 initialPosition;
    private InputAction action;

    void Start()
    {
        action = InputManager.GetInputAction(actionName);
        if (action == null)
        {
            Debug.LogError("Input action not found.");
            return;
        }

        action.Enable();
        action.performed += OnAction;

        initialPosition = transform.position;
    }

    void OnAction(InputAction.CallbackContext _)
    {
        if (rb == null)
        {
            Debug.LogError("Rigidbody2D reference is missing.");
            return;
        }

        rb.linearVelocity = Vector2.zero;
        transform.position = initialPosition;
    }

    void OnDestroy()
    {
        if (action == null) return;

        action.Disable();
        action.performed -= OnAction;
    }
}
