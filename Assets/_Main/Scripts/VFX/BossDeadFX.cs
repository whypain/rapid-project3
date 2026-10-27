using System.Threading;
using Cysharp.Threading.Tasks;
using PrimeTween;
using UnityEngine;

public class BossDeadFX : MonoBehaviour, IResetable
{
    [SerializeField] private GameObject[] explosions;
    [SerializeField] private float explosionInterval = 0.5f;

    private CancellationTokenSource cts = new CancellationTokenSource();

    private void OnEnable()
    {
        PlayExplosion().Forget();
    }

    public async UniTask PlayExplosion()
    {
        Tween.ShakeCamera(Camera.main, 1f, 3f, 20f);

        foreach (var explosion in explosions)
        {
            explosion.gameObject.SetActive(true);
            await UniTask.Delay(System.TimeSpan.FromSeconds(explosionInterval), cancellationToken: cts.Token);

            if (cts.Token.IsCancellationRequested)
            {
                DisableAllExplosions();
                break;
            }
        }
    }

    private void DisableAllExplosions()
    {
        foreach (var explosion in explosions)
        {
            explosion.gameObject.SetActive(false);
        }
    }

    public void Reset()
    {
        cts?.Cancel();
        cts = new CancellationTokenSource();

        DisableAllExplosions();
    }
}
