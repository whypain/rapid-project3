using Cysharp.Threading.Tasks;
using UnityEngine;

public class SkillLaser : Skill
{
    [SerializeField] private GameObject laserPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private Transform parent;

    private GameObject currentLaser;

    protected override async UniTask OnUse()
    {
        currentLaser = Instantiate(laserPrefab, firePoint.position, firePoint.rotation, parent);
        await WaitForLaserToFinish();
    }

    private async UniTask WaitForLaserToFinish()
    {
        while (currentLaser != null)
        {
            await UniTask.Yield();
        }
    }
}
