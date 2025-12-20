using UnityEngine;

public class GameplayReportService : SubService
{
    private ReportUIController reportUIController;

    public override void Initialize(GameplayService gameplayService)
    {
        base.Initialize(gameplayService);
        reportUIController = gameplayService.View.ReportUIController;
        
        // Listen to week ended event
        GameplayEvents.OnWeekEnded += OnWeekEnded;
        GameplayEvents.OnPlayerBankrupt += OnPlayerBankrupt;
    }

    public override void Start()
    {
    }

    public override void Tick()
    {
    }

    public override void Dispose()
    {
        GameplayEvents.OnWeekEnded -= OnWeekEnded;
        GameplayEvents.OnPlayerBankrupt -= OnPlayerBankrupt;
    }

    private void OnWeekEnded()
    {
        ShowReport();
    }

    private void OnPlayerBankrupt()
    {
        ShowReport();
    }

    private void ShowReport()
    {
        Debug.Log("[GameplayReportService] Showing report...");
        
        var sessionData = gameplayService.ActiveSessionData;
        reportUIController.Show();
        reportUIController.Initialize(sessionData, OnReportConfirmed);
    }

    private void OnReportConfirmed()
    {
        Debug.Log("[GameplayReportService] Report confirmed. Session ended.");
        reportUIController.Hide();
        
        // TODO: Back to main menu atau restart
    }
}
