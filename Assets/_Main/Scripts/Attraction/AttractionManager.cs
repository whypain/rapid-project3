using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

[DefaultExecutionOrder(-1)]
public class AttractionManager : MonoBehaviour, IPointOfInterestsProvider
{
    public static AttractionManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private List<IPointOfInterest> pointsOfInterest = new List<IPointOfInterest>();

    public void RegisterPOI(IPointOfInterest poi)
    {
        if (!pointsOfInterest.Contains(poi))
        {
            pointsOfInterest.Add(poi);
        }
        Debug.Log($"POI registered: {poi.GetType().Name}");
    }

    public void UnregisterPOI(IPointOfInterest poi)
    {
        if (pointsOfInterest.Contains(poi))
        {
            pointsOfInterest.Remove(poi);
        }
        Debug.Log($"POI unregistered: {poi.GetType().Name}");
    }

    public IPointOfInterest[] GetPointsOfInterests()
    {
        return pointsOfInterest.ToArray();
    }
}

public interface IPointOfInterest
{
    float GetPriority();
    SplineContainer GetSpline();
}

public interface IPointOfInterestsProvider
{
    IPointOfInterest[] GetPointsOfInterests();
    void RegisterPOI(IPointOfInterest poi);
    void UnregisterPOI(IPointOfInterest poi);
}
