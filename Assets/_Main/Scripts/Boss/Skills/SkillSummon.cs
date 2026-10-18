using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class SkillSummon : Skill
{
    [SerializeField] private GameObject summonPrefab;
    [SerializeField] private Transform summonParent;
    [SerializeField] private Transform[] summonPoints;
    [SerializeField] private int summonCount = 1;

    public override bool CanUse()
    {
        int childCount = summonParent.childCount;
        return base.CanUse() && childCount < summonCount;
    }

    protected override async UniTask OnUse()
    {
        if (summonPoints.Length == 0 || summonPrefab == null) return;
        if (summonCount > summonPoints.Length) summonCount = summonPoints.Length;

        for (int i = 0; i < summonCount; i++)
        {
            int pointIndex = UnityEngine.Random.Range(0, summonPoints.Length);
            Instantiate(summonPrefab, summonPoints[pointIndex].position, summonPoints[pointIndex].rotation, summonParent);
            await UniTask.Delay(TimeSpan.FromSeconds(0.5f));
        }
        await UniTask.Yield();
    }
}
