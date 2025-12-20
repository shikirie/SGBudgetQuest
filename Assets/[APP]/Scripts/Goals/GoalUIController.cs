using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GoalUIController : BaseController
{
    [SerializeField] private TMP_Text textDescription;
    [SerializeField] private GoalItemUI goalItemPrefab;
    [SerializeField] private Transform goalListContainer;
    [SerializeField] private ToggleGroup goalToggleGroup;
    [SerializeField] private Button buttonConfirm;

    private GoalData selectedGoal;
    private Action<GoalData> onGoalSelected;
    private GoalData[] goals;

    protected override void Awake()
    {
        base.Awake();
        buttonConfirm.onClick.AddListener(HandleOnButtonConfirmClicked);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        buttonConfirm.onClick.RemoveListener(HandleOnButtonConfirmClicked);
    }

    public void Initialize(float initialAllowance, GoalData[] goals, Action<GoalData> onGoalSelected)
    {
        this.goals = goals;
        this.onGoalSelected = onGoalSelected;
        textDescription.text = $"You have <color=#38A239><b><size=45>S${initialAllowance:F1}</size></b></color> for the week.\nPick an item you want to buy at the end of the week!";
        LoadGoals();
    }

    private void LoadGoals()
    {
        foreach (GoalData goal in goals)
        {
            GoalItemUI goalItem = Instantiate(goalItemPrefab, goalListContainer);
            goalItem.Initialize(goal, HandleOnGoalSelected);
            goalItem.SetToggleGroup(goalToggleGroup);
        }

        goalToggleGroup.SetAllTogglesOff(false);
        selectedGoal = null;
    }

    private void HandleOnGoalSelected(GoalData data)
    {
        selectedGoal = data;
    }

    private void HandleOnButtonConfirmClicked()
    {
        if (goalToggleGroup.AnyTogglesOn() == false)
            return;
            
        if (selectedGoal != null)
        {
            onGoalSelected?.Invoke(selectedGoal);
        }
    }
}
