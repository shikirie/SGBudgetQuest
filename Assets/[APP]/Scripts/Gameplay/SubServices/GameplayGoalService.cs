using UnityEngine;

public class GameplayGoalService : SubService
{
    private GoalUIController goalUIController;
    private GoalData selectedGoal;

    public override void Initialize(GameplayService gameplayService)
    {
        base.Initialize(gameplayService);
        goalUIController = gameplayService.View.GoalUIController;
        
        GameplayEvents.OnGameRestarted += OnGameRestarted;
    }

    public override void Start()
    {
        ShowGoalSelection();
    }

    public override void Tick()
    {
    }

    public override void Dispose()
    {
        GameplayEvents.OnGameRestarted -= OnGameRestarted;
    }

    private void OnGameRestarted()
    {
        ShowGoalSelection();
    }

    private void ShowGoalSelection()
    {
        float initialAllowance = gameplayService.ActiveSessionData.InitialAllowance;
        GoalData[] allGoals = gameplayService.ActiveGoalData.GetAllGoalData().ToArray();
        
        goalUIController.Show();
        goalUIController.Initialize(initialAllowance, allGoals, OnGoalConfirmed);
    }

    private void OnGoalConfirmed(GoalData goal)
    {
        selectedGoal = goal;
        goalUIController.Hide();
        
        Debug.Log($"[GameplayGoalService] Goal selected: {goal.GoalName}");
        
        gameplayService.ActiveSessionData.StartNewSession(selectedGoal);
        
        GameplayEvents.OnGoalSelected?.Invoke();
    }
}
