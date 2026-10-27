using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class SkillSummon : Skill, IResetable
{
    [SerializeField] private float summonDelay = 0.5f;
    [SerializeField] private Summoning[] summons;

    [SerializeField] private SoundConfig summonSound;

    CancellationTokenSource summonCts;

    public override bool CanUse()
    {
        bool allSummonsActive = true;
        foreach (var summon in summons)
        {
            if (summon != null && !summon.IsActive)
            {
                allSummonsActive = false;
                break;
            }
        }
        return base.CanUse() && !allSummonsActive;
    }

    public void Reset()
    {
        summonCts?.Cancel();
        summonCts = null;
    }

    protected override async UniTask OnUse()
    {
        if (summons.Length == 0) return;

        summonCts?.Cancel();
        summonCts = new CancellationTokenSource();

        for (int i = 0; i < summons.Length; i++)
        {
            if (summonCts.IsCancellationRequested) break;

            if (summons[i] == null || summons[i].IsActive) continue;

            summons[i].Activate();
            SoundManager.Instance.PlaySoundFromPool(summonSound);
            await UniTask.Delay(TimeSpan.FromSeconds(summonDelay), cancellationToken: summonCts.Token);
        }
        await UniTask.Yield();
    }
}
