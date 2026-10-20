using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class SkillSummon : Skill
{
    [SerializeField] private GameObject summonPrefab;
    [SerializeField] private float summonDelay = 0.5f;
    [SerializeField] private Transform summonParent;
    [SerializeField] private Transform[] summonPoints;

    private Dictionary<Transform, GameObject> activeSummons = new Dictionary<Transform, GameObject>();

    public override bool CanUse()
    {
        int activeCount = 0;
        foreach (var summon in activeSummons.Values)
        {
            if (summon != null)
            {
                activeCount++;
            }
        }
        return base.CanUse() && activeCount < summonPoints.Length;
    }

    protected override async UniTask OnUse()
    {
        if (summonPoints.Length == 0 || summonPrefab == null) return;

        for (int i = 0; i < summonPoints.Length; i++)
        {
            if (activeSummons.ContainsKey(summonPoints[i]) && activeSummons[summonPoints[i]] != null) continue;

            GameObject newSummon = Instantiate(summonPrefab, summonPoints[i].position, summonPoints[i].rotation, summonParent);
            activeSummons[summonPoints[i]] = newSummon;
            await UniTask.Delay(TimeSpan.FromSeconds(summonDelay));
        }
        await UniTask.Yield();
    }
}
