using UnityEngine;
using UnityEngine.Splines;

public class Attraction : MonoBehaviour, IPointOfInterest
{
    [SerializeField] private SplineContainer splineContainer;
    [SerializeField] private float priority;

    private void OnEnable()
    {
        AttractionManager.Instance.RegisterPOI(this);
    }

    private void OnDisable()
    {
        AttractionManager.Instance.UnregisterPOI(this);
    }

    public float GetPriority()
    {
        return priority;
    }

    public SplineContainer GetSpline()
    {
        return splineContainer;
    }
}