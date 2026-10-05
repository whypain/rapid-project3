using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Splines;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private SplineAnimate splineAnimate;
    [SerializeField] private InputActionReference moveActionReference;
    [SerializeField] private InputActionReference resetActionReference;

    private Route route;

    private void Update()
    {
        if (moveActionReference.action.triggered && !splineAnimate.IsPlaying)
        {
            route = PlayerRouteController.CalculateRoute(AttractionManager.Instance);

            splineAnimate.Container = route.routeSplineContainer;
            splineAnimate.Play();
        }

        if (resetActionReference.action.triggered)
        {
            Debug.Log("Resetting");
            splineAnimate.Restart(false);
            splineAnimate.Container = null;
        }
    }
}
