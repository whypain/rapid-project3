using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PowerUpUI : MonoBehaviour
{
    public string PowerUpName => powerUpNameText.text;

    [SerializeField] private Image powerUpIcon;
    [SerializeField] private TMP_Text powerUpNameText;
    [SerializeField] private TMP_Text powerUpCountText;
    [SerializeField] private TMP_Text buttonPromptText;

    public void SetPowerUpInfo(string name, int count, string buttonPrompt)
    {
        powerUpNameText.text = name;

        if (count <= 0)
        {
            powerUpCountText.text = "";
        }
        else
        {
            powerUpCountText.text = $"x{count}";
        }

        buttonPromptText.text = $"[ {buttonPrompt} ]";
    }

    public void SetIcon(Sprite icon)
    {
        if (powerUpIcon != null)
        {
            powerUpIcon.sprite = icon;
        }
    }
}
