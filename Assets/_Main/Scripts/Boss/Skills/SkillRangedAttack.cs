using Cysharp.Threading.Tasks;
using UnityEngine;

public class SkillRangedAttack : Skill
{
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform parent;
    [SerializeField] private Transform firePoint;

    protected override async UniTask OnUse()
    {
        Instantiate(projectilePrefab, firePoint.position, firePoint.rotation, parent);
        await UniTask.Yield();
    }
}
