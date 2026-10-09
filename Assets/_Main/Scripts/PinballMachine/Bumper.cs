using UnityEngine;

public class Bumper : MonoBehaviour, ICollisionEffect
{
    public void OnHit(Ball ball)
    {
        // Handle the bumper hit logic here
    }
}

public interface ICollisionEffect
{
    void OnHit(Ball ball);
}