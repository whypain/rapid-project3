using UnityEngine;

public class BossAnimator : MonoBehaviour
{
    [SerializeField] private Animator anim;

    public void SetBossPhase(int phase)
    {
        Debug.Log($"Setting boss phase to {phase}");
        anim.SetInteger("Phase", phase);
    }

    public void PlaySkillAnimation(string skillName)
    {
        Debug.Log($"Playing skill animation: {skillName}");
        anim.SetTrigger(skillName);
    }

    public void StopAnimation(string skillName)
    {
        Debug.Log($"Stopping skill animation: {skillName}");
        anim.SetTrigger(skillName + "Done");
    }

    public void CancelAnimation(string skillName)
    {
        Debug.Log($"Cancelling skill animation: {skillName}");
        anim.SetTrigger(skillName + "Cancel");
    }
}
