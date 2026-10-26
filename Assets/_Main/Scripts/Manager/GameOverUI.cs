using PrimeTween;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private Button toMenuButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private CanvasGroup canvasGroup;

    private void OnEnable()
    {
        restartButton.onClick.AddListener(OnRestartButtonClicked);
        toMenuButton.onClick.AddListener(OnToMenuButtonClicked);

        canvasGroup.alpha = 0f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        Tween.Alpha(canvasGroup, 1f, 0.5f, useUnscaledTime: true);
    }

    private void OnDisable()
    {
        restartButton.onClick.RemoveListener(OnRestartButtonClicked);
        toMenuButton.onClick.RemoveListener(OnToMenuButtonClicked);
    }

    private void OnRestartButtonClicked()
    {
        ResetManager.Instance.Reset();
        GameState.Instance.SetInGame();
    }

    private void OnToMenuButtonClicked()
    {
        GameState.Instance.SetGameMenu();
    }
}