using Modules;
using UnityEngine;

[CreateAssetMenu(fileName = "NewGoalData", menuName = "[APP]/Data/GoalData")]
public class GoalData : ScriptableObject
{
    [Tooltip("Stable ID derived from the asset name. Rename the asset to change it.")]
    [SerializeField, ReadOnly] private string goalId;
    public string GoalId => string.IsNullOrEmpty(goalId) ? name : goalId;

    [Header("Display Properties")]
    [SerializeField] private string goalName;
    public string GoalName => goalName;
    [SerializeField] private Sprite goalIcon;
    public Sprite GoalIcon => goalIcon;
    [SerializeField, TextArea(2, 4)] private string goalDescription;
    public string GoalDescription => goalDescription;
    [SerializeField] private float targetPrice;
    public float TargetPrice => targetPrice;
    [SerializeField] private GoalDifficulty difficulty;
    public GoalDifficulty Difficulty => difficulty;

#if UNITY_EDITOR
    private void OnValidate()
    {
        goalId = name;
    }
#endif
}
