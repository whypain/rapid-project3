using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class BossDeadFX : MonoBehaviour, IResetable
{
    [SerializeField] private GameObject[] explosions;
    [SerializeField] private float explosionInterval = 0.5f;
    [SerializeField] private float disableIn = 0.5f;

    private CancellationTokenSource cts = new CancellationTokenSource();

    private void OnEnable()
    {
        PlayExplosion().Forget();
    }

    public async UniTask PlayExplosion()
    {
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
    }
}
