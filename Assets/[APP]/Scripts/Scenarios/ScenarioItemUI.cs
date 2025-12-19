using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScenarioItemUI : BaseItemUI
{
    [SerializeField] private TMP_Text textHappiness;
    private ChoiceData choiceData;
    private Action<ChoiceData> onSelectedCallback;

    public void Initialize(ChoiceData data, Action<ChoiceData> onSelected)
    {
        onSelectedCallback = onSelected;
        choiceData = data;
        SetupUI();
    }

    public void SetupUI()
    {
        if (choiceData == null) return;

        textTitle.text = choiceData.ChoiceText;
        imageIcon.sprite = choiceData.ChoiceIcon;
        textPrice.text = $"S${choiceData.ChoiceCost:F2}";
        string happinessSign = choiceData.ChoiceHappiness > 0 ? "+" : "-";
        textHappiness.text = $"Happiness {happinessSign}{choiceData.ChoiceHappiness}";
    }

    protected override void HandleOnToggleValueChanged(bool isOn)
    {
        if (choiceData == null) return;

        if (isOn)
        {
            onSelectedCallback?.Invoke(choiceData);
        }
    }
}
