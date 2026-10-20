using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class SkillToggleCollider : Skill
{
    [SerializeField] private Collider2D collider;
    [SerializeField] private float sweepDuration = 1f;

    protected override async UniTask OnUse()
    {
        collider.enabled = true;
        await UniTask.Delay(TimeSpan.FromSeconds(sweepDuration));
        collider.enabled = false;
    }

    public override void Cancel()
    {
        collider.enabled = false;
        base.Cancel();
    }
}
