using Modules.CurrencyBarUISystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatusUIController : BaseController
{
    [SerializeField] private TMP_Text textDay;
    [SerializeField] private CurrencyBarUI currencyWalletUI;
    [SerializeField] private CurrencyBarUI currencyHappinessUI;
    [SerializeField] private Image happinessIcon;
    [SerializeField] private Sprite[] happinessSprite;

    public void SetDayText(string dayText)
    {
        textDay.text = dayText;
    }

    public void SetWalletValue(float value, bool immidiate = false)
    {
        if (immidiate)
        {
            currencyWalletUI.SetValueImmediate(value);
        }
        else
        {
            if (value == 0) return;
            currencyWalletUI.ShowChange(value);
            currencyWalletUI.SetValue(currencyWalletUI.GetValue() + value);
        }
    }

    public void SetHappinessValue(float value, bool immidiate = false)
    {
        if (immidiate)
        {
            float clampedValue = Mathf.Clamp(value, 0, 100);
            currencyHappinessUI.SetValueImmediate(clampedValue);
        }
        else
        {
            if (value == 0) return;
            currencyHappinessUI.ShowChange(value);
            float current = currencyHappinessUI.GetValue();
            float newValue = Mathf.Clamp(current + value, 0, 100);
            currencyHappinessUI.SetValue(newValue);
        }

        SetupHappinessIcon();
    }

    private void SetupHappinessIcon()
    {
        float happinessValue = currencyHappinessUI.GetValue();

        if (happinessIcon == null || happinessSprite == null || happinessSprite.Length == 0)
        {
            return;
        }

        int spriteIndex;

        if (happinessValue >= 100)
        {
            spriteIndex = 0;
        }
        else if (happinessValue >= 75)
        {
            spriteIndex = 1;
        }
        else if (happinessValue >= 50)
        {
            spriteIndex = 2;
        }
        else if (happinessValue >= 25)
        {
            spriteIndex = 3;
        }
        else
        {
            spriteIndex = happinessSprite.Length - 1;
        }

        spriteIndex = Mathf.Clamp(spriteIndex, 0, happinessSprite.Length - 1);
        happinessIcon.sprite = happinessSprite[spriteIndex];
    }
}
