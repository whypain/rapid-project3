using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class SkillCharge : Skill
{
    [SerializeField] private float chargeDuration = 5f;
    [SerializeField] private Skill nextSkill;

    private CancellationTokenSource cancelTokenSource;

    protected override async UniTask OnUse()
    {
        cancelTokenSource = new CancellationTokenSource();
        await UniTask.Delay(TimeSpan.FromSeconds(chargeDuration), cancellationToken: cancelTokenSource.Token);
        if (cancelTokenSource.IsCancellationRequested) return;

        nextSkill.Use().Forget();
    }

    public override void Cancel()
    {
        cancelTokenSource.Cancel();
        base.Cancel();
    }
}
