using UnityEngine;

[System.Serializable]
public abstract class PowerUp
{
    public abstract string Name { get; }

    public static int id;

    protected int instanceId;
    protected PowerUpSystem powerUpSystem;
    protected float duration;
    protected float durationLeft;
    protected PowerUp(
        PowerUpSystem system,
        float durationSeconds)
    {
        instanceId = id++;
        powerUpSystem = system;
        duration = Mathf.Max(0f, durationSeconds);
        durationLeft = duration;
    }
    public virtual void Update(float deltaTime)
    {
        durationLeft -= deltaTime;
        Debug.Log($"PowerUp {Name} (ID: {instanceId}) duration left: {durationLeft}");
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
