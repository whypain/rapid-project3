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

        Cancel();
        await nextSkill.Use();
    }

    public override void Cancel()
    {
        owner.SkillCancelled(this);
        cancelTokenSource.Cancel();
        base.Cancel();
    }
}
