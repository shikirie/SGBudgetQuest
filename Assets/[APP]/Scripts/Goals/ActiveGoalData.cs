using System;
using System.Collections.Generic;
using SimpleJSON;
using Modules.SavingSystems;

[Serializable]
public class ActiveGoalData : ISaveable
{
    private readonly GoalDatabase goalDatabase;

    public ActiveGoalData(GoalDatabase goalDatabase)
    {
        this.goalDatabase = goalDatabase;
    }

    #region Public API

    /// <summary>
    /// Gets the static data for a specific goal by ID.
    /// </summary>
    public GoalData GetGoalData(string goalId) => goalDatabase.GetItem(goalId);

    /// <summary>
    /// Gets all static goal data from the database.
    /// </summary>
    public List<GoalData> GetAllGoalData() => new List<GoalData>(goalDatabase.GetAllItems());

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
