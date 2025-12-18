using Modules.SavingSystems;
using SimpleJSON;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class ProjectSavingSystem : SavingSystem, IStartable, ITickable
{
    // [Inject] private readonly ActiveGoalData activeGoalData;
    // [Inject] private readonly ActiveScenarioData activeScenarioData;
    // [Inject] private readonly ActiveSessionData activeSessionData;

    private int progress;
    private bool isSaving;
    private bool isLoading;
    private System.Action onFinished;

    /// <summary>
    /// Clears all runtime data and initializes to default state.
    /// Call this when starting a new game.
    /// </summary>
    public void ClearPreviousData()
    {
        // activeGoalData.Initialize();
        // activeScenarioData.Initialize();
        // Note: ActiveSessionData is initialized when starting a new run, not here
    }

    /// <summary>
    /// Resets runtime data for retry scenarios while keeping progression data.
    /// Call this when retrying a run (surrender retry or restart).
    /// </summary>
    public void ResetForRetry(float startingKuld = 0)
    {
        ClearPreviousData();
    }

    /// <summary>
    /// Saves all game state to the specified slot.
    /// </summary>
    public void SaveAll(System.Action onFinished = null)
    {
        isSaving = true;
        this.onFinished = onFinished;
        progress = 3; // Number of things we're saving

        try
        {
            // SaveToFile($"{nameof(activeGoalData)}",
            //     activeGoalData.AsJSON(),
            //     () => progress--);

            // SaveToFile($"{nameof(activeScenarioData)}",
            //     activeScenarioData.AsJSON(),
            //     () => progress--);

            // SaveToFile($"{nameof(activeSessionData)}",
            //     activeSessionData.AsJSON(),
            //     () => progress--);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error while saving data: {e}");
            isSaving = false;
        }
    }

    /// <summary>
    /// Loads all game state from the specified slot.
    /// </summary>
    public void LoadAll(int slot = 0, System.Action onFinished = null)
    {
        isLoading = true;
        this.onFinished = onFinished;
        progress = 3; // Number of things we're loading

        try
        {
            // LoadFromFile($"{nameof(activeGoalData)}",
            //     result =>
            //     {
            //         if (result != null && !result.IsNull)
            //         {
            //             activeGoalData.LoadFromJSON(result);
            //         }
            //         progress--;
            //     });

            // LoadFromFile($"{nameof(activeScenarioData)}",
            //     result =>
            //     {
            //         if (result != null && !result.IsNull)
            //         {
            //             activeScenarioData.LoadFromJSON(result);
            //         }
            //         progress--;
            //     });

            // LoadFromFile($"{nameof(activeSessionData)}",
            //     result =>
            //     {
            //         if (result != null && !result.IsNull)
            //         {
            //             activeSessionData.LoadFromJSON(result);
            //         }
            //         progress--;
            //     });
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error while loading data: {e}");
            isLoading = false;
        }
    }

    void IStartable.Start()
    {
    }

    void ITickable.Tick()
    {
        CheckCompletion();
    }

    private void CheckCompletion()
    {
        if ((isSaving || isLoading) && progress == 0)
        {
            onFinished?.Invoke();
            isSaving = false;
            isLoading = false;
            onFinished = null;
        }
    }
}