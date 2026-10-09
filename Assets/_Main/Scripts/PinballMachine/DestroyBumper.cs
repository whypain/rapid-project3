using UnityEngine;

public class DestroyBumper : MonoBehaviour, ICollisionEffect
{
    public void OnHit(Ball ball)
    {
        ball.ReleaseBall();
    }
}
