using UnityEngine;

public class DamageCollider : MonoBehaviour, ITriggerEffect
{
    public void OnHit(Ball ball)
    {
        DamageSystem.Instance.RegisterDamagedBall(ball);
    }
}
