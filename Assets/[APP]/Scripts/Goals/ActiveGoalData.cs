using System;
using System.Collections.Generic;

[Serializable]
public class ActiveGoalData
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
}
