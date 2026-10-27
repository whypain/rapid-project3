using UnityEngine;

public class ActiveMonitor : MonoBehaviour
{
    private void OnEnable()
    {
        Debug.Log("Enabled");
    }

    private void OnDisable()
    {
        Debug.Log("Disabled");
    }
}
