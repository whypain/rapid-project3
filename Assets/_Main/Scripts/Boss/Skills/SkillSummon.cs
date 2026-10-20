using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class SkillSummon : Skill
{
    [SerializeField] private float summonDelay = 0.5f;
    [SerializeField] private Summoning[] summons;

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

    protected override async UniTask OnUse()
    {
        if (summons.Length == 0) return;

        for (int i = 0; i < summons.Length; i++)
        {
            if (summons[i] == null || summons[i].IsActive) continue;

            summons[i].Activate();
            await UniTask.Delay(TimeSpan.FromSeconds(summonDelay));
        }
        await UniTask.Yield();
    }
}
