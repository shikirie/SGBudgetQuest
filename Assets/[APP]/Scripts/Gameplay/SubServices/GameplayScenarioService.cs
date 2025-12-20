using System.Collections.Generic;
using UnityEngine;

public class GameplayScenarioService : SubService
{
    private ScenarioUIController scenarioUIController;
    private bool sessionStarted = false;
    private const int TOTAL_DAYS = 7;
    private List<ScenarioData> weekScenarios;

    public override void Initialize(GameplayService gameplayService)
    {
        base.Initialize(gameplayService);
        scenarioUIController = gameplayService.View.ScenarioUIController;
        
        GameplayEvents.OnBudgetConfirmed += OnBudgetConfirmed;
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
        GameplayEvents.OnBudgetConfirmed -= OnBudgetConfirmed;
        GameplayEvents.OnGameRestarted -= OnGameRestarted;
    }

    private void OnGameRestarted()
    {
        // Reset state for new session
        sessionStarted = false;
        weekScenarios = null;
    }

    private void OnBudgetConfirmed()
    {
        StartScenario();
    }

    private void StartScenario()
    {
        sessionStarted = true;
        
        // Load all scenarios untuk week ini (day 0-6)
        LoadWeekScenarios();
        
        // Check bankruptcy before starting scenarios
        if (gameplayService.ActiveSessionData.CheckBankruptcy())
        {
            Debug.LogWarning("[GameplayScenarioService] Player is BANKRUPT at start! Ending session...");
            HandleBankruptcy();
            return;
        }
        
        Debug.Log("[GameplayScenarioService] Starting scenario gameplay...");
        
        // Trigger session started event untuk notify status service dll
        GameplayEvents.OnScenarioStarted?.Invoke();
        
        // Show first scenario
        ShowNextScenario();
    }

    private void LoadWeekScenarios()
    {
        List<ScenarioData> allScenarios = gameplayService.ActiveScenarioData.GetAllScenarioData();
        
        // Filter scenarios berdasarkan dayIndex 0-6
        weekScenarios = new List<ScenarioData>();
        for (int i = 0; i < allScenarios.Count; i++)
        {
            if (allScenarios[i].DayIndex >= 0 && allScenarios[i].DayIndex < TOTAL_DAYS)
            {
                weekScenarios.Add(allScenarios[i]);
            }
        }
        
        // Sort scenarios by dayIndex (bubble sort)
        for (int i = 0; i < weekScenarios.Count - 1; i++)
        {
            for (int j = 0; j < weekScenarios.Count - i - 1; j++)
            {
                if (weekScenarios[j].DayIndex > weekScenarios[j + 1].DayIndex)
                {
                    ScenarioData temp = weekScenarios[j];
                    weekScenarios[j] = weekScenarios[j + 1];
                    weekScenarios[j + 1] = temp;
                }
            }
        }
        
        Debug.Log($"[GameplayScenarioService] Loaded {weekScenarios.Count} scenarios for the week");
    }

    private void ShowNextScenario()
    {
        int currentDayIndex = gameplayService.ActiveSessionData.GetCurrentDay() - 1;
        
        // Check bankruptcy before showing scenario
        if (gameplayService.ActiveSessionData.CheckBankruptcy())
        {
            Debug.LogWarning("[GameplayScenarioService] Player is BANKRUPT before showing scenario! Ending session...");
            HandleBankruptcy();
            return;
        }
        
        if (currentDayIndex >= TOTAL_DAYS)
        {
            Debug.LogWarning("[GameplayScenarioService] Week completed, no more scenarios.");
            return;
        }

        // Cari scenario untuk day ini
        ScenarioData todayScenario = null;
        for (int i = 0; i < weekScenarios.Count; i++)
        {
            if (weekScenarios[i].DayIndex == currentDayIndex)
            {
                todayScenario = weekScenarios[i];
                break;
            }
        }
        
        if (todayScenario == null)
        {
            Debug.LogWarning($"[GameplayScenarioService] No scenario found for day {currentDayIndex}, skipping...");
            gameplayService.ActiveSessionData.IncrementDay();
            
            if (gameplayService.ActiveSessionData.GetCurrentDay() - 1 >= TOTAL_DAYS)
            {
                EndWeek();
            }
            else
            {
                ShowNextScenario();
            }
            return;
        }

        // Update status day display (tidak increment karena sudah di day yang benar)
        var statusService = gameplayService.GetService<GameplayStatusService>();
        if (statusService != null)
        {
            statusService.UpdateDayDisplay();
        }

        // Show scenario UI
        scenarioUIController.Show();
        scenarioUIController.Initialize(todayScenario, OnChoiceSelected);
        
        // Update button text
        bool isLastDay = (gameplayService.ActiveSessionData.GetCurrentDay() - 1) >= (TOTAL_DAYS - 1);
        scenarioUIController.SetButtonConfirmText(isLastDay ? "End Week" : "Continue");
        
        Debug.Log($"[GameplayScenarioService] Showing scenario for Day {gameplayService.ActiveSessionData.GetCurrentDay()}: {todayScenario.QuestionTitle}");
    }

    private void OnChoiceSelected(ChoiceData choice)
    {
        Debug.Log($"[GameplayScenarioService] Choice selected: {choice.ChoiceText} (Cost: S${choice.ChoiceCost}, Happiness: {choice.ChoiceHappiness})");
        
        // Update session data dengan choice
        var sessionData = gameplayService.ActiveSessionData;
        
        // Check if player can afford the choice
        if (choice.ChoiceCost > 0 && sessionData.SessionSummary.currentWallet < choice.ChoiceCost)
        {
            Debug.LogWarning("[GameplayScenarioService] Player cannot afford this choice! Setting wallet to 0 and ending session...");
            sessionData.SessionSummary.currentWallet = 0;
            scenarioUIController.Hide();
            HandleBankruptcy();
            return;
        }
        
        sessionData.SpendFromWallet(choice);
        sessionData.UpdateHappiness(choice);
        
        // Update UI status
        var statusService = gameplayService.GetService<GameplayStatusService>();
        if (statusService != null)
        {
            statusService.UpdateWallet(-choice.ChoiceCost);
            statusService.UpdateHappiness(choice.ChoiceHappiness);
        }
        
        // Check bankruptcy
        if (sessionData.CheckBankruptcy())
        {
            Debug.LogWarning("[GameplayScenarioService] Player is BANKRUPT! Ending session...");
            scenarioUIController.Hide();
            HandleBankruptcy();
            return;
        }
        
        // Hide scenario UI
        scenarioUIController.Hide();
        
        // Move to next day
        gameplayService.ActiveSessionData.IncrementDay();
        
        // Check if week is complete
        if (gameplayService.ActiveSessionData.GetCurrentDay() - 1 >= TOTAL_DAYS)
        {
            EndWeek();
        }
        else
        {
            // Show next scenario
            ShowNextScenario();
        }
    }

    private void HandleBankruptcy()
    {
        Debug.Log("[GameplayScenarioService] Handling bankruptcy - sending data and showing report...");
        
        // Get session data as JSON
        var sessionData = gameplayService.ActiveSessionData;
        string jsonData = sessionData.ToJson();
        
        Debug.Log($"[GameplayScenarioService] Bankruptcy Session Data JSON: {jsonData}");
        
        // Send to AWS using APIManager
        if (gameplayService.APIManager != null)
        {
            CoroutineRunner.Run(gameplayService.APIManager.PostRequest(jsonData));
        }
        else
        {
            Debug.LogError("[GameplayScenarioService] APIManager not found! Cannot send data to AWS.");
        }
        
        // Trigger bankruptcy event untuk notify report service
        GameplayEvents.OnPlayerBankrupt?.Invoke();
    }

    private void EndWeek()
    {
        Debug.Log("[GameplayScenarioService] Week completed! Sending data to AWS...");
        
        // Get session data as JSON
        var sessionData = gameplayService.ActiveSessionData;
        string jsonData = sessionData.ToJson();
        
        Debug.Log($"[GameplayScenarioService] Session Data JSON: {jsonData}");
        
        // Send to AWS using APIManager
        if (gameplayService.APIManager != null)
        {
            CoroutineRunner.Run(gameplayService.APIManager.PostRequest(jsonData));
        }
        else
        {
            Debug.LogError("[GameplayScenarioService] APIManager not found! Cannot send data to AWS.");
        }
        
        // Trigger week ended event untuk notify report service
        GameplayEvents.OnWeekEnded?.Invoke();
    }

    public bool IsSessionStarted() => sessionStarted;
    public int GetCurrentDay() => gameplayService.ActiveSessionData.GetCurrentDay();
}
