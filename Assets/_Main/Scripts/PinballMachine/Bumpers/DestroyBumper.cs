using UnityEngine;

public class DestroyBumper : MonoBehaviour, ICollisionEffect
{
    public void OnHit(Ball ball, Collision2D collision)
    {
        ball.ReleaseBall();
    }
}
