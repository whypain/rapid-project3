using UnityEngine;

public class Bumper : MonoBehaviour, ICollisionEffect
{
    [SerializeField] private SoundConfig bumperHit;
    [SerializeField] private SoundConfig bumperDestroyed;

    public virtual void OnHit(Ball ball, Collision2D collision)
    {
        ball.OnHit();
        SoundManager.Instance.PlaySoundFromPool(bumperHit);
    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
        SoundManager.Instance.PlaySoundFromPool(bumperDestroyed);
    }
}

public interface ICollisionEffect
{
    void OnHit(Ball ball, Collision2D collision);
}

public interface ITriggerEffect
{
    void OnHit(Ball ball);
}