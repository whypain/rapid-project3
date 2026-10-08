using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public static class InputManager
{
    private static Dictionary<string, InputAction> inputMaps = new()
    {
        { "LeftHand",  new InputAction("LeftHand", InputActionType.Button,  "<Keyboard>/f") },
        { "RightHand", new InputAction("RightHand", InputActionType.Button, "<Keyboard>/j") },
        { "ResetBall", new InputAction("ResetBall", InputActionType.Button, "<Keyboard>/r") }
    };

    public static InputAction GetInputAction(string actionName)
    {
        if (inputMaps.TryGetValue(actionName, out InputAction action))
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
