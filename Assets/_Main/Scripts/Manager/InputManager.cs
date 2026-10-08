using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }
    [SerializeField] private InputActionAsset actionMap;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
    }

    public InputAction GetInputAction(string actionName)
    {
        InputAction action = actionMap.FindAction(actionName);
        if (action != null)
        {
            return action;
        }
        else
        {
            Debug.LogError($"Input action '{actionName}' not found.");
            return null;
        }
    }
}
