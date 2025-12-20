using System;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ReportUIController : BaseController
{
    [Header("Header")]
    [SerializeField] private SpriteColorChanger spriteColorChangerHeader;

    [Header("Goal")]
    [SerializeField] private Image imageGoalIcon;
    [SerializeField] private TMP_Text textGoalNameAndCost;
    [SerializeField] private SpriteChanger spriteChangerGoalStatus;

    [Header("Status")]
    [SerializeField] private TMP_Text textStatusSavings;
    [SerializeField] private TMP_Text textStatusHappiness;

    [Header("Session Summary - Ledger")]
    [SerializeField] private TMP_Text textLedger;

    [Header("Session Summary - Expenditures")]
    [SerializeField] private TMP_Text textExpendituresLeft;
    [SerializeField] private TMP_Text textExpendituresRight;

    [Header("Session Summary - Auditor Logs")]
    [SerializeField] private TMP_Text textAuditorLogs;

    [Header("Buttons")]
    [SerializeField] private Button buttonConfirm;

    [Header("Colors")]
    [SerializeField] private Color colorRed;
    [SerializeField] private Color colorGreen;

    // Constants
    private const int HAPPINESS_PEAK_THRESHOLD = 80;
    private const int HAPPINESS_STABLE_THRESHOLD = 50;
    private const int HAPPINESS_DRAINED_THRESHOLD = 30;
    private const int LEAK_CATEGORY_FONT_SIZE = 35;
    private const string CURRENCY_SYMBOL = "S$";
    private const string MONEY_FORMAT = "F1";

    private Action onConfirmCallback;
    private ActiveSessionData sessionData;

    protected override void Awake()
    {
        base.Awake();
        buttonConfirm.onClick.AddListener(OnConfirmButtonClicked);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        buttonConfirm.onClick.RemoveListener(OnConfirmButtonClicked);
    }

    public void Initialize(ActiveSessionData sessionData, Action onConfirmCallback)
    {
        if (sessionData == null)
        {
            Debug.LogError("[ReportUIController] Initialize failed: sessionData is null.");
            return;
        }

        this.onConfirmCallback = onConfirmCallback;
        this.sessionData = sessionData;

        SessionDataSummary reportData = sessionData.SessionSummary;
        GoalData currentGoal = sessionData.CurrentGoal;
        bool isWin = IsWinningSession(reportData.sessionStatus);

        UpdateHeader(isWin);
        UpdateGoal(currentGoal, isWin);
        UpdateStatus(reportData, isWin);
        UpdateLedger(reportData);
        UpdateExpenditures(reportData);
        UpdateAuditorLogs();
    }

    private bool IsWinningSession(string sessionStatus)
    {
        if (sessionStatus == SessionStatus.Bankrupt.ToString())
        {
            return false;
        }
        
        return sessionStatus == SessionStatus.Perfect.ToString() || 
               sessionStatus == SessionStatus.Good.ToString();
    }

    private void UpdateHeader(bool isWin)
    {
        spriteColorChangerHeader.ChangeColor(isWin ? 0 : 1);
    }

    private void UpdateGoal(GoalData currentGoal, bool isWin)
    {
        imageGoalIcon.sprite = currentGoal.GoalIcon;
        textGoalNameAndCost.text = $"{currentGoal.GoalName}: <b>{CURRENCY_SYMBOL}{currentGoal.TargetPrice.ToString(MONEY_FORMAT)}</b>";
        spriteChangerGoalStatus.ChangeSprite(isWin ? 0 : 1);
    }

    private void UpdateStatus(SessionDataSummary reportData, bool isWin)
    {
        Color savingsColor = isWin ? colorGreen : colorRed;
        textStatusSavings.text = $"Your Savings\n<color={ToHex(savingsColor)}><b>{CURRENCY_SYMBOL}{reportData.currentSavings.ToString(MONEY_FORMAT)}</b></color>";
        
        Color happinessColor = GetSanityColor(reportData.currentHappiness);
        string happinessLabel = GetSanityLabel(reportData.currentHappiness);
        textStatusHappiness.text = $"Happiness: <color={ToHex(happinessColor)}><b>{happinessLabel} ({reportData.currentHappiness}/100)</b></color>";
    }

    private void UpdateLedger(SessionDataSummary reportData)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"<color={ToHex(colorGreen)}>+ {CURRENCY_SYMBOL}{reportData.initialAllowance.ToString(MONEY_FORMAT)}</color>");
        sb.AppendLine($"<color={ToHex(colorRed)}>- {CURRENCY_SYMBOL}{reportData.totalWalletSpent.ToString(MONEY_FORMAT)}</color>");
        sb.AppendLine();
        sb.Append($"<b>{CURRENCY_SYMBOL}{reportData.currentWallet.ToString(MONEY_FORMAT)}</b>");
        textLedger.text = sb.ToString();
    }

    private void UpdateExpenditures(SessionDataSummary reportData)
    {
        bool isWantsHigher = reportData.weeklySpentWants > reportData.weeklySpentNeeds;
        Color wantsColor = isWantsHigher ? colorRed : Color.black;
        string wantsColorHex = ToHex(wantsColor);

        var sbLeft = new StringBuilder();
        sbLeft.AppendLine("Needs");
        sbLeft.AppendLine($"<color={wantsColorHex}>Wants</color>");
        sbLeft.AppendLine();
        sbLeft.Append($"<size={LEAK_CATEGORY_FONT_SIZE}>Biggest Leak Category: <uppercase>{sessionData.GetLeakiestCategory()}</uppercase></size>");

        var sbRight = new StringBuilder();
        sbRight.AppendLine($"{CURRENCY_SYMBOL}{reportData.weeklySpentNeeds.ToString(MONEY_FORMAT)}");
        sbRight.AppendLine($"<color={wantsColorHex}>{CURRENCY_SYMBOL}{reportData.weeklySpentWants.ToString(MONEY_FORMAT)}</color>");
        sbRight.AppendLine();
        sbRight.Append($"<size={LEAK_CATEGORY_FONT_SIZE}>{sessionData.GetLeakiestCategoryValue().ToString(MONEY_FORMAT)}</size>");

        textExpendituresLeft.text = sbLeft.ToString();
        textExpendituresRight.text = sbRight.ToString();
    }

    private void UpdateAuditorLogs()
    {
        textAuditorLogs.text = GenerateAuditorLog();
    }

    private void OnConfirmButtonClicked()
    {
        onConfirmCallback?.Invoke();
    }

    private string GetSanityLabel(int happiness)
    {
        if (happiness >= HAPPINESS_PEAK_THRESHOLD) return "PEAK";
        if (happiness >= HAPPINESS_STABLE_THRESHOLD) return "STABLE";
        if (happiness >= HAPPINESS_DRAINED_THRESHOLD) return "DRAINED";
        return "CRITICAL";
    }

    private Color GetSanityColor(int happiness)
    {
        return happiness >= HAPPINESS_STABLE_THRESHOLD ? colorGreen : colorRed;
    }

    private string GenerateAuditorLog()
    {
        SessionStatus status = sessionData.GetSessionStatus();
        SpendCategory biggestLeak = sessionData.GetLeakiestCategory();

        // Special message for Bankrupt
        if (status == SessionStatus.Bankrupt)
        {
            return GetBankruptMessage(biggestLeak);
        }

        if (status == SessionStatus.Perfect)
        {
            return "STATUS: GOD TIER. Wallet safe, Sanity intact. Perfect run.";
        }

        string statusText = GetStatusText(status, biggestLeak);
        string adviceText = GetAdviceText(status, biggestLeak);
        
        return $"{statusText} {adviceText}";
    }

    private string GetBankruptMessage(SpendCategory biggestLeak)
    {
        string bankruptText = "STATUS: BANKRUPT! You ran out of money mid-week.";
        
        string cause = biggestLeak switch
        {
            SpendCategory.Food => "Your food budget devoured your wallet.",
            SpendCategory.Transport => "You spent all your cash on rides. Should've walked.",
            SpendCategory.Social => "FOMO drained your account. Friends are expensive.",
            SpendCategory.Shopping => "Shopping spree? More like financial suicide.",
            _ => "You mismanaged every penny."
        };

        return $"{bankruptText} {cause} Game Over.";
    }

    private string GetStatusText(SessionStatus status, SpendCategory biggestLeak)
    {
        return status switch
        {
            SessionStatus.Good => "STATUS: BURNOUT DETECTED. Goal secured, but you look like a zombie.",
            SessionStatus.Average => "STATUS: TASK FAILED. You bought too much happiness. Goal locked.",
            _ => "STATUS: CRITICAL FAILURE. Broke AND Depressed. Big Oof."
        };
    }

    private string GetAdviceText(SessionStatus status, SpendCategory biggestLeak)
    {
        if (status == SessionStatus.Good && biggestLeak == SpendCategory.None)
        {
            return "You saved too hard. Live a little.";
        }

        return biggestLeak switch
        {
            SpendCategory.Food => "Stop drinking your calories.",
            SpendCategory.Transport => "Walk more, ride less. Your legs are free.",
            SpendCategory.Social => "FOMO is expensive. Learn to say No.",
            SpendCategory.Shopping => "Retail therapy just broke your bank.",
            SpendCategory.None => "Life is expensive. Essential costs hit hard.",
            _ => "Check your budgeting strategy."
        };
    }

    private static string ToHex(Color c) => "#" + ColorUtility.ToHtmlStringRGB(c);
}
