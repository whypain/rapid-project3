using UnityEngine;

public class Bumper : MonoBehaviour, ICollisionEffect
{
    public void OnHit(Ball ball, Collision2D collision)
    {
        ball.OnHit();
    }
}

public interface ICollisionEffect
{
    void OnHit(Ball ball, Collision2D collision);
}