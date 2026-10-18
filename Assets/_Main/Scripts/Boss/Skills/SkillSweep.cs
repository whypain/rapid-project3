using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class SkillSweep : Skill
{
    [SerializeField] private Collider2D sweepCollider;
    [SerializeField] private float sweepDuration = 1f;

    protected override async UniTask OnUse()
    {
        sweepCollider.enabled = true;
        await UniTask.Delay(TimeSpan.FromSeconds(sweepDuration));
        sweepCollider.enabled = false;
    }
}
