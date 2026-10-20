using UnityEngine;

public class Summoning : MonoBehaviour, IResetable
{
    public bool IsActive => gameObject.activeSelf;

    public void Activate()
    {
        gameObject.SetActive(true);
    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
    }

    public void Reset()
    {
        Deactivate();
    }
}