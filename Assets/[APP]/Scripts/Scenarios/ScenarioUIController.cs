using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScenarioUIController : BaseController
{
    [SerializeField] private TMP_Text textTitle;
    [SerializeField] private TMP_Text textSubtitle;
    [SerializeField] private TMP_Text textDescription;
    [SerializeField] private ScenarioItemUI scenarioItemPrefab;
    [SerializeField] private Transform scenarioListContainer;
    [SerializeField] private ToggleGroup scenarioToggleGroup;
    [SerializeField] private Button buttonConfirm;
    [SerializeField] private TMP_Text textButtonConfirm;

    private ScenarioData currentScenario;
    private ChoiceData selectedChoice;
    private Action<ChoiceData> onChoiceSelected;

    protected override void Awake()
    {
        base.Awake();
        buttonConfirm.onClick.AddListener(HandleButtonConfirmClicked);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        buttonConfirm.onClick.RemoveListener(HandleButtonConfirmClicked);
    }

    public void Initialize(ScenarioData scenario, Action<ChoiceData> onChoiceSelected)
    {
        currentScenario = scenario;
        this.onChoiceSelected = onChoiceSelected;

        SetupUI();
    }

    private void SetupUI()
    {
        if (currentScenario == null)
            return;

        textTitle.text = currentScenario.QuestionTitle;
        textSubtitle.text = currentScenario.QuestionSubtitle;
        textDescription.text = currentScenario.QuestionText;

        LoadChoices();
    }

    private void LoadChoices()
    {
        foreach (ChoiceData choice in currentScenario.Choices)
        {
            ScenarioItemUI scenarioItem = Instantiate(scenarioItemPrefab, scenarioListContainer);
            scenarioItem.Initialize(choice, HandleOnChoiceSelected);
            scenarioItem.SetToggleGroup(scenarioToggleGroup);
        }

        scenarioToggleGroup.SetAllTogglesOff(false);
        selectedChoice = null;
    }

    public void SetButtonConfirmText(string text)
    {
        textButtonConfirm.text = text;
    }

    private void HandleOnChoiceSelected(ChoiceData data)
    {
        selectedChoice = data;
    }

    private void HandleButtonConfirmClicked()
    {
        if (selectedChoice != null)
        {
            onChoiceSelected?.Invoke(selectedChoice);
        }
    }
}
