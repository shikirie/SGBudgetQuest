using System;
using System.Collections.Generic;

[Serializable]
public class ActiveScenarioData
{
    private readonly ScenarioDatabase scenarioDatabase;

    public ActiveScenarioData(ScenarioDatabase scenarioDatabase)
    {
        this.scenarioDatabase = scenarioDatabase;
    }

    #region Public API

    /// <summary>
    /// Gets the static data for a specific scenario by ID.
    /// </summary>
    public ScenarioData GetScenarioData(string scenarioId) => scenarioDatabase.GetItem(scenarioId);

    /// <summary>
    /// Gets all static scenario data from the database.
    /// </summary>
    public List<ScenarioData> GetAllScenarioData() => new List<ScenarioData>(scenarioDatabase.GetAllItems());

    #endregion
}
