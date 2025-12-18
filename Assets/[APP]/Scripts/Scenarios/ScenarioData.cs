using System;
using Modules;
using UnityEngine;

[CreateAssetMenu(fileName = "NewScenarioData", menuName = "[APP]/Data/ScenarioData")]
public class ScenarioData : ScriptableObject
{
    [Tooltip("Stable ID derived from the asset name. Rename the asset to change it.")]
    [SerializeField, ReadOnly] private string scenarioId;
    public string ScenarioId => string.IsNullOrEmpty(scenarioId) ? name : scenarioId;
    [SerializeField] private int dayIndex;
    public int DayIndex => dayIndex;

    [Header("Display Properties")]
    [SerializeField] private string questionTitle;
    public string QuestionTitle => questionTitle;
    [SerializeField] private string questionText;
    public string QuestionText => questionText;
    [SerializeField] private Sprite questionIcon;
    public Sprite QuestionIcon => questionIcon;

    [Header("Choices")]
    [SerializeField] private ChoiceData[] choices;
    public ChoiceData[] Choices => choices;

#if UNITY_EDITOR
    private void OnValidate()
    {
        scenarioId = name;
    }
#endif
}

[Serializable]
public class ChoiceData
{
    [SerializeField] private string choiceText;
    public string ChoiceText => choiceText;
    [SerializeField] private float choiceCost;
    public float ChoiceCost => choiceCost;
    [SerializeField] private int choiceHappiness;
    public int ChoiceHappiness => choiceHappiness;
    [SerializeField] private string choiceHappinessText;
    public string ChoiceHappinessText => choiceHappinessText;
    [SerializeField] private SpendCategory spendCategory;
    public SpendCategory SpendCategory => spendCategory;
    [SerializeField] private SpendType spendType;
    public SpendType SpendType => spendType;
    [SerializeField] private Sprite choiceIcon;
    public Sprite ChoiceIcon => choiceIcon;
}
