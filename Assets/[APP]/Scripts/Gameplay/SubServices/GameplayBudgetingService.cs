using UnityEngine;

public class GameplayBudgetingService : SubService
{
    private BudgetingUIController budgetingUIController;
    private float savingsAmount;
    private float walletAmount;

    public override void Initialize(GameplayService gameplayService)
    {
        base.Initialize(gameplayService);
        budgetingUIController = gameplayService.View.BudgetingUIController;
        
        GameplayEvents.OnGoalSelected += OnGoalSelected;
    }

    public override void Start()
    {
    }

    public override void Tick()
    {
    }

    public override void Dispose()
    {
        GameplayEvents.OnGoalSelected -= OnGoalSelected;
    }

    private void OnGoalSelected()
    {
        GoalData selectedGoal = gameplayService.ActiveSessionData.CurrentGoal;
        float initialAllowance = gameplayService.ActiveSessionData.InitialAllowance;
        
        budgetingUIController.Show();
        budgetingUIController.Initialize(initialAllowance, selectedGoal, OnBudgetConfirmed);
    }

    private void OnBudgetConfirmed(float savings, float wallet)
    {
        savingsAmount = savings;
        walletAmount = wallet;
        
        budgetingUIController.Hide();
        
        Debug.Log($"[GameplayBudgetingService] Budget confirmed - Savings: S${savings:F1}, Wallet: S${wallet:F1}");
        
        gameplayService.ActiveSessionData.AddToSavings(savings);
        gameplayService.ActiveSessionData.AddToWallet(wallet);
        
        GameplayEvents.OnBudgetConfirmed?.Invoke();
    }
}
