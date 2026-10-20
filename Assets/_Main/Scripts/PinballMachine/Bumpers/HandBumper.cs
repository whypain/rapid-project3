using UnityEngine;

public class HandBumper : ForceBumper
{
    public override void OnHit(Ball ball, Collision2D collision)
    {
        base.OnHit(ball, collision);
        DamageSystem.Instance.UnregisterDamagedBall(ball);
    }
}
