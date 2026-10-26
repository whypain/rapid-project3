using UnityEngine;

public class Wind : PowerUp
{
    private bool isActive;

    public Wind(
        PowerUpSystem system,
        float durationSeconds)
        : base(system, durationSeconds)
    {
    }

    public override string Name => "wind";

    public void ActivateWindBarrier(GameObject barrier)
    {
        if (isActive || powerUpSystem == null)
            return;

        isActive = true;
        powerUpSystem.AcquireWindBarrier();
    }

    public override void OnExpire()
    {
        if (!isActive)
            return;

        isActive = false;
        powerUpSystem?.ReleaseWindBarrier();
    }
}
