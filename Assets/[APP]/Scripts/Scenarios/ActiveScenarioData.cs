using System;
using System.Collections.Generic;
using SimpleJSON;
using Modules.SavingSystems;

[Serializable]
public class ActiveScenarioData : ISaveable
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

    #region Serialization (ISaveable)

    public JSONNode AsJSON()
    {
        throw new NotImplementedException();
    }

    public void LoadFromJSON(JSONNode json)
    {
        throw new NotImplementedException();
    }

    #endregion
}
