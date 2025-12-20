using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BudgetingUIController : BaseController
{
    [SerializeField] private TMP_Text textAllowance;
    [SerializeField] private Image imageGoalIcon;
    [SerializeField] private TMP_Text textGoalTarget;
    [SerializeField] private TMP_Text textSavings;
    [SerializeField] private TMP_Text textWallet;
    [SerializeField] private Slider sliderBudgeting;
    [SerializeField] private TMP_Text textAlert;
    [SerializeField] private Button buttonConfirm;

    private float initialAllowance;
    private GoalData selectedGoal;
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

    public void Initialize(float initialAllowance, GoalData selectedGoal, Action<float, float> onBudgetConfirmed)
    {
        this.initialAllowance = initialAllowance;
        this.selectedGoal = selectedGoal;
        this.onBudgetConfirmed = onBudgetConfirmed;
        SetupUI();
    }

    private void SetupUI()
    {
        sliderBudgeting.minValue = 0f;
        sliderBudgeting.maxValue = initialAllowance;
        sliderBudgeting.value = initialAllowance / 2;
        textAllowance.text = $"S${initialAllowance:F1}";
        if (selectedGoal != null)
        {
            imageGoalIcon.sprite = selectedGoal.GoalIcon;
            textGoalTarget.text = $"S${selectedGoal.TargetPrice:F1}";
        }
        else
        {
            imageGoalIcon.sprite = null;
            textGoalTarget.text = "No Goal Selected";
        }

        UpdateUI();
    }

    private void UpdateUI()
    {
        savingsAmount = sliderBudgeting.value;
        walletAmount = initialAllowance - savingsAmount;

        textSavings.text = $"<size=40>Savings</size>\nS${savingsAmount:F1}";

        textWallet.text = $"<size=40>Wallet</size>\nS${walletAmount:F1}";

        if (selectedGoal != null && savingsAmount < selectedGoal.TargetPrice)
        {
            textAlert.text = "NOT ENOUGH! Increase savings to reach your goal.";
            textAlert.color = new Color(0.882353f, 0.4f, 0.3529412f);
        }
        else if (walletAmount < 10)
        {
            textAlert.text = "GOAL SECURED, but you'll be starving!";
            textAlert.color = new Color(1f, 0.5f, 0f);
        }
        else
        {
            textAlert.text = "LOOKS GOOD! Ready to survive the week?";
            textAlert.color = new Color(0.2196078f, 0.6352941f, 0.2237839f);
        }
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
