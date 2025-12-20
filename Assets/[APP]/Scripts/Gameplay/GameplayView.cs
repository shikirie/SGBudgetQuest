using UnityEngine;

public class GameplayView : MonoBehaviour
{
    [SerializeField] private GoalUIController goalUIController;
    [SerializeField] private BudgetingUIController budgetingUIController;
    [SerializeField] private ScenarioUIController scenarioUIController;
    [SerializeField] private StatusUIController statusUIController;
    [SerializeField] private ReportUIController reportUIController;

    public GoalUIController GoalUIController => goalUIController;
    public BudgetingUIController BudgetingUIController => budgetingUIController;
    public ScenarioUIController ScenarioUIController => scenarioUIController;
    public StatusUIController StatusUIController => statusUIController;
    public ReportUIController ReportUIController => reportUIController;
}
