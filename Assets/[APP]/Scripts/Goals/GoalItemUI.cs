using System;
using TMPro;
using UnityEngine;

public class GoalItemUI : BaseItemUI
{
    [SerializeField] private TMP_Text textDifficulty;

    private GoalData goalData;
    private Action<GoalData> onSelectedCallback;

    public void Initialize(GoalData data, Action<GoalData> onSelected)
    {
        onSelectedCallback = onSelected;
        goalData = data;
        SetupUI();
    }

    public void SetupUI()
    {
        if (goalData == null) return;

        textTitle.text = goalData.GoalName;
        imageIcon.sprite = goalData.GoalIcon;
        textPrice.text = $"<size=30>PRICE</size>\nS${goalData.TargetPrice:F2}";
        textDifficulty.text = goalData.Difficulty.ToString();
    }

    protected override void HandleOnToggleValueChanged(bool isOn)
    {
        if (goalData == null) return;

        if (isOn)
        {
            onSelectedCallback?.Invoke(goalData);
        }
    }
}
