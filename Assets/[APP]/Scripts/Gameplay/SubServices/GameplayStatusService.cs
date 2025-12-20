using System;
using UnityEngine;

public class GameplayStatusService : SubService
{
    private StatusUIController statusUIController;

    public override void Initialize(GameplayService gameplayService)
    {
        this.gameplayService = gameplayService;
        statusUIController = gameplayService.View.StatusUIController;
        
        GameplayEvents.OnScenarioStarted += OnScenarioStarted;
        GameplayEvents.OnGameRestarted += OnGameRestarted;
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
        GameplayEvents.OnGameRestarted -= OnGameRestarted;
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
        statusUIController.SetDayText($"Day {currentDay}/7");
    }

    public void UpdateWallet(float amount, bool immediate = false)
    {
        statusUIController.SetWalletValue(amount, immediate);
    }

    public void UpdateHappiness(float value, bool immediate = false)
    {
        statusUIController.SetHappinessValue(value, immediate);
    }

    private void OnGameRestarted()
    {
        statusUIController.Hide();
    }
}
