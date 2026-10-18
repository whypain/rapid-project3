using UnityEngine.Events;

[System.Serializable]
public class BossPhase
{
    public int healthThreshold;
    public Skill openningSkill;
    public Skill[] skills;
    public UnityEvent onPhaseStart;
}
