using UnityEngine;

public class GameplayStatusService : SubService
{
    private StatusUIController statusUIController;

    public override void Initialize(GameplayService gameplayService)
    {
        this.gameplayService = gameplayService;
        statusUIController = gameplayService.View.StatusUIController;
        
        // Listen to session events
        GameplayEvents.OnScenarioStarted += OnScenarioStarted;
    }

    public override void Start()
    {
    }

    public override void Tick()
    {
    }

    public override void Dispose()
    {
        GameplayEvents.OnScenarioStarted -= OnScenarioStarted;
    }

    private void OnScenarioStarted()
    {
        // Initialize status UI when session starts
        var sessionData = gameplayService.ActiveSessionData;
        
        statusUIController.Show();
        UpdateDayDisplay();
        statusUIController.SetWalletValue(sessionData.SessionSummary.currentWallet, true);
        statusUIController.SetHappinessValue(sessionData.SessionSummary.currentHappiness, true);
        
        Debug.Log("[GameplayStatusService] Status UI initialized for new session");
    }

    public void UpdateDayDisplay()
    {
        int currentDay = gameplayService.ActiveSessionData.GetCurrentDay();
        statusUIController.SetDayText($"Day {currentDay}");
    }

    public void UpdateWallet(float amount, bool immediate = false)
    {
        statusUIController.SetWalletValue(amount, immediate);
    }

    public void UpdateHappiness(float value, bool immediate = false)
    {
        statusUIController.SetHappinessValue(value, immediate);
    }

    public int GetCurrentDay() => gameplayService.ActiveSessionData.GetCurrentDay();
}
