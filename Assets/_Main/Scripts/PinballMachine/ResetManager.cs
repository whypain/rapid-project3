using UnityEngine;
using UnityEngine.InputSystem;

public interface IResetable
{
    void Reset();
}

public class ResetManager : MonoBehaviour
{
    [SerializeField] private InputActionReference actionRef;
    private InputAction action;

    private void Start()
    {
        action = actionRef.action;

        action.Enable();
        action.performed += OnReset;
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
        action.performed -= OnReset;
        action.Disable();
    }
}
