using UnityEngine;

public class Wind : PowerUp
{
    private bool isActive;

    public Wind(
        PowerUpSystem system,
        Ball targetBall,
        float durationSeconds)
        : base(system, targetBall, durationSeconds)
    {
    }
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
