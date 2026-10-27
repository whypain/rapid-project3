using Microsoft.Extensions.Logging.Abstractions;
using UnityEngine;
using UnityEngine.InputSystem;

public interface IResetable
{
    void Reset();
}

public class ResetManager : MonoBehaviour
{
    public static ResetManager Instance { get; private set; } 

    [SerializeField] private InputActionReference actionRef;
    private InputAction action;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        if (actionRef == null) return;

        action = actionRef.action;

        action.Enable();
        action.performed += OnReset;
    }

    public void Reset()
    {
        OnReset(new InputAction.CallbackContext());
    }

    private void OnReset(InputAction.CallbackContext _)
    {
        foreach (var resetable in FindObjectsByType<MonoBehaviour>())
        {
            if (resetable is IResetable resetableComponent)
            {
                resetableComponent.Reset();
            }
        }
    }

    void OnDestroy()
    {
        if (action == null) return;
        action.performed -= OnReset;
        action.Disable();
    }
}
