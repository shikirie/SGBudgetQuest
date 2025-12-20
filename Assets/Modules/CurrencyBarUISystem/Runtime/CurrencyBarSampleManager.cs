using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Modules.CurrencyBarUISystem
{
    public class CurrencyBarSampleManager : MonoBehaviour
    {
        [SerializeField] private CurrencyBarUI currencyBarUI;
        [SerializeField] private Button buttonPlus;
        [SerializeField] private Button buttonMinus;

        private void Awake()
        {
            buttonPlus.onClick.AddListener(HandleOnButtonPlusClicked);
            buttonMinus.onClick.AddListener(HandleOnButtonMinusClicked);
            currencyBarUI.SetValueImmediate(0);
        }

        private void OnDestroy()
        {
            buttonPlus.onClick.RemoveListener(HandleOnButtonPlusClicked);
            buttonMinus.onClick.RemoveListener(HandleOnButtonMinusClicked);
        }

        private void HandleOnButtonPlusClicked()
        {
            float randomValue = Random.Range(1, 5);
            currencyBarUI.ShowChange(randomValue);
            currencyBarUI.SetValue(currencyBarUI.GetValue() + randomValue);
        }

        private void HandleOnButtonMinusClicked()
        {
            float randomValue = Random.Range(-5, -1);
            currencyBarUI.ShowChange(randomValue);
            currencyBarUI.SetValue(currencyBarUI.GetValue() + randomValue);
        }
    }


}

