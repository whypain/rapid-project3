using UnityEngine;

[System.Serializable]
public abstract class PowerUp
{
    public abstract string Name { get; }

    protected PowerUpSystem powerUpSystem;
    protected float duration;
    protected float durationLeft;
    protected PowerUp(
        PowerUpSystem system,
        float durationSeconds)
    {
        powerUpSystem = system;
        duration = Mathf.Max(0f, durationSeconds);
        durationLeft = duration;
    }
    public virtual void Update(float deltaTime)
    {
        durationLeft -= deltaTime;
    }
    public virtual void OnHit(Ball ball, Collision2D collision)
    {
        ball.OnHit();
    }
    public bool IsExpired()
    {
        return durationLeft <= 0;
    }
    public virtual void OnExpire()
    {
        // ใช้คืนสถานะเมื่อบัฟหมดเวลา
    }
}
