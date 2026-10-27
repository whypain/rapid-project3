using PrimeTween;
using UnityEngine;

public class ScaleOnEnable : MonoBehaviour
{
    [SerializeField] private float initialScale = 0.5f;
    [SerializeField] private float targetScale = 1f;
    [SerializeField] private float duration = 0.5f;
    [SerializeField] private Ease ease;

    private void OnEnable()
    {
        Tween.LocalScale(gameObject.transform, initialScale * Vector3.one, targetScale * Vector3.one, duration, ease);
    }
}