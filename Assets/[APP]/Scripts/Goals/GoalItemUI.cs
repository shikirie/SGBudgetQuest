using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GoalItemUI : MonoBehaviour
{
    [SerializeField] private Toggle toggle;
    [SerializeField] private TMP_Text textTitle;
    [SerializeField] private Image imageIcon;
    [SerializeField] private TMP_Text textPrice;

    private GoalData goalData;
    private Action<GoalData> onSelectedCallback;

    private void Awake()
    {
        toggle.onValueChanged.AddListener(HandleOnToggleValueChanged);
    }

    private void OnDestroy()
    {
        toggle.onValueChanged.RemoveListener(HandleOnToggleValueChanged);
    }

    public void Initialize(GoalData data, Action<GoalData> onSelected)
    {
        onSelectedCallback = onSelected;
        goalData = data;
        SetupUI();
    }

    public void SetToggleGroup(ToggleGroup group)
    {
        toggle.group = group;
    }

    public void SetupUI()
    {
        if (goalData == null) return;

        textTitle.text = goalData.GoalName;
        imageIcon.sprite = goalData.GoalIcon;
        textPrice.text = $"<size=30>PRICE</size>\nS${goalData.TargetPrice:F2}";
    }

    private void HandleOnToggleValueChanged(bool isOn)
    {
        if (goalData == null) return;

        if (isOn)
        {
            onSelectedCallback?.Invoke(goalData);
        }
    }
}
