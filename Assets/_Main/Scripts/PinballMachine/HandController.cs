using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

public class HandController : MonoBehaviour
{
    [SerializeField] private float angle = 30f;
    [SerializeField] private float swingDuration = 0.2f;
    [SerializeField] private float bounciness = 3f;

    [Header("Input")]
    [SerializeField] private InputActionReference actionReference;

    [Header("References")]
    [SerializeField] private Transform handAnchor;
    [SerializeField] private PhysicsMaterial2D physMat;
    [SerializeField] private Collider2D collider;
    [SerializeField] private ForceBumper bumper;

    private Quaternion initialRotation;
    private CancellationTokenSource swingCts;
    private InputAction action;

    void Start()
    {
        action = actionReference?.action; 
        if (action == null)
        {
            Debug.LogError("Input action not found.");
            return;
        }

        action.Enable();
        //action.performed += OnAction;
        action.performed += OnActionPressed;
        action.canceled += OnActionReleased;

        initialRotation = handAnchor.localRotation;
    }


    async void OnAction(InputAction.CallbackContext _)
    {
        if (collider == null)
        {
            Debug.LogError("Collider reference is missing.");
            return;
        }

        bumper.Activate();
        await HandleInputCanceled();
        bumper.Deactivate();

        // prevent the hand from slapping the ball downward if it clips through the ball during the swing
        collider.enabled = false;

        // return to initial rotation
        await QLerp(handAnchor.localRotation, initialRotation, CancellationToken.None);

        collider.enabled = true;
    }
    async void OnActionPressed(InputAction.CallbackContext _)
    {
        if (collider == null || handAnchor == null || physMat == null) return;
        if (swingCts != null)
        {
            swingCts.Cancel();
            swingCts.Dispose();
        }
        swingCts = new CancellationTokenSource();
        bumper.Activate();
        physMat.bounciness = bounciness;
        Quaternion targetRotation = initialRotation * Quaternion.Euler(new Vector3(0f, 0f, angle));
        await QLerp(handAnchor.localRotation, targetRotation, swingCts.Token);
        physMat.bounciness = 0f;
        bumper.Deactivate();
    }
    private async void OnActionReleased(InputAction.CallbackContext _)
    {
        if (collider == null || handAnchor == null || physMat == null) return;
        if (swingCts != null)
        {
            swingCts.Cancel();
            swingCts.Dispose();
            swingCts = null;
        }
        bumper.Deactivate();
        physMat.bounciness = 0f;
        collider.enabled = false;
        await QLerp(handAnchor.localRotation, initialRotation, CancellationToken.None);
        collider.enabled = true;
    }

    private async Task HandleInputCanceled()
    {
        if (swingCts == null)
        {
            swingCts = new CancellationTokenSource();
        }
        if (handAnchor == null)
        {
            Debug.LogError("Hand anchor is not assigned.");
            return;
        }
        if (physMat == null)
        {
            Debug.LogError("Physics material is not assigned.");
            return;
        }


        physMat.bounciness = bounciness;

        await QLerp(handAnchor.localRotation, initialRotation * Quaternion.Euler(new Vector3(0f, 0f, angle)), swingCts.Token);

        physMat.bounciness = 0f;


        if (swingCts == null) return;
        swingCts.Cancel();
        swingCts.Dispose();
        swingCts = null;
    }

    private async Task QLerp(Quaternion from, Quaternion to, CancellationToken token)
    {
        float timeElapsed = 0f;
        while (timeElapsed < swingDuration && !token.IsCancellationRequested)
        {
            float t = timeElapsed / swingDuration;
            handAnchor.localRotation = Quaternion.Lerp(from, to, t);
            timeElapsed += Time.deltaTime;

            await Task.Yield();
        }

        handAnchor.localRotation = to;
    }
    void OnDestroy()
    {
        if (action == null) return;

        action.Disable();
        //action.performed -= OnAction;
        action.performed -= OnActionPressed;
        action.canceled -= OnActionReleased;

        if (swingCts != null)
        {
            swingCts.Cancel();
            swingCts.Dispose();
            swingCts = null;
        }

        if (handAnchor != null)
        {
            handAnchor.localRotation = initialRotation;
        }
    }
}
