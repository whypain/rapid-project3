using System.Linq;
using UnityEngine;
using UnityEngine.Splines;

public class PlayerRouteController : MonoBehaviour
{
    public static Route CalculateRoute(IPointOfInterestsProvider provider)
    {
        if (provider == null)
        {
            Debug.LogError("Provider is null. Cannot calculate route.");
            return null;
        }

        IPointOfInterest[] pointsOfInterest = provider.GetPointsOfInterests();
        if (pointsOfInterest == null || pointsOfInterest.Length == 0)
        {
            Debug.LogWarning("No points of interest available. Returning player's current position as the route.");
            return null;
        }


        IPointOfInterest closest = pointsOfInterest.OrderBy(p => p.GetPriority()).First();
        if (closest == null)
        {
            Debug.LogWarning("No valid points of interest found. Returning player's current position as the route.");
            return null;
        }
        return new Route(closest.GetSpline());
    }
}

public class Route
{
    public SplineContainer routeSplineContainer;

    public Route(SplineContainer spline)
    {
        routeSplineContainer = spline;
    }
}