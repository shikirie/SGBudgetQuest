using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BudgetingUIController : BaseController
{
    [SerializeField] private TMP_Text textAllowance;
    [SerializeField] private TMP_Text textSavings;
    [SerializeField] private TMP_Text textWallet;
    [SerializeField] private Slider sliderBudgeting;
    [SerializeField] private Button buttonConfirm;

    private float initialAllowance;
    private float savingsAmount;
    private float walletAmount;
    private Action<float, float> onBudgetConfirmed;

    protected override void Awake()
    {
        base.Awake();
        sliderBudgeting.onValueChanged.AddListener(HandleOnSliderValueChanged);
        buttonConfirm.onClick.AddListener(HandleOnButtonConfirmClicked);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        sliderBudgeting.onValueChanged.RemoveListener(HandleOnSliderValueChanged);
        buttonConfirm.onClick.RemoveListener(HandleOnButtonConfirmClicked);
    }

    public void Initialize(float initialAllowance, Action<float, float> onBudgetConfirmed)
    {
        this.initialAllowance = initialAllowance;
        this.onBudgetConfirmed = onBudgetConfirmed;
        SetupUI();
    }

    private void SetupUI()
    {
        sliderBudgeting.minValue = 0f;
        sliderBudgeting.maxValue = initialAllowance;
        sliderBudgeting.value = initialAllowance / 2;
        textAllowance.text = $"S${initialAllowance:F2}";

        UpdateUI();
    }

    private void UpdateUI()
    {
        savingsAmount = sliderBudgeting.value;
        walletAmount = initialAllowance - savingsAmount;

        textSavings.text = $"<size=40>Savings</size>\nS${savingsAmount:F2}";

        textWallet.text = $"<size=40>Wallet</size>\nS${walletAmount:F2}";
    }

    private void HandleOnSliderValueChanged(float value)
    {
        UpdateUI();
    }

    private void HandleOnButtonConfirmClicked()
    {
        onBudgetConfirmed?.Invoke(savingsAmount, walletAmount);
    }
}
