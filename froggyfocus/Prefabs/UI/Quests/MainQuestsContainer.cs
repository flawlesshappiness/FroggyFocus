using Godot;

public partial class MainQuestsContainer : ControlScript
{
    [Export]
    public Control MainContainer;

    [Export]
    public Label LabelPartner;

    [Export]
    public Label LabelManager;

    [Export]
    public Label LabelScientist;

    [Export]
    public Control PartnerControl;

    [Export]
    public Control ManagerControl;

    [Export]
    public Control ScientistControl;

    private const int PARTNER_STEP_MAX = 5;
    private const int MANAGER_STEP_MAX = 5;
    private const int SCIENTIST_STEP_MAX = 4;

    private bool PartnerFinished => MainQuestController.Instance.GetPartnerStep() >= PARTNER_STEP_MAX;
    private bool ManagerFinished => MainQuestController.Instance.GetManagerStep() >= MANAGER_STEP_MAX;
    private bool ScientistFinished => MainQuestController.Instance.GetScientistStep() >= SCIENTIST_STEP_MAX;

    protected override void Initialize()
    {
        base.Initialize();
        GameFlagsController.Instance.OnFlagChanged += FlagChanged;
        GameProfileController.Instance.OnGameProfileSelected += GameProfileSelected;
        Load();
    }

    protected override void OnShow()
    {
        base.OnShow();
        var intro_finished = GameFlags.IsFlag(LetterScene.INTRO_LETTERS_ID, 1);
        var quests_finished = PartnerFinished && ManagerFinished && ScientistFinished;
        MainContainer.Visible = intro_finished && !quests_finished;
    }

    private void GameProfileSelected(int profile)
    {
        Load();
    }

    private void Load()
    {
        PartnerQuestAdvanced(MainQuestController.Instance.GetPartnerStep());
        ManagerQuestAdvanced(MainQuestController.Instance.GetManagerStep());
        ScientistQuestAdvanced(MainQuestController.Instance.GetScientistStep());
    }

    private void FlagChanged(string id, int step)
    {
        if (id == MainQuestController.PARTNER_QUEST_ID)
        {
            PartnerQuestAdvanced(step);
        }
        else if (id == MainQuestController.MANAGER_QUEST_ID)
        {
            ManagerQuestAdvanced(step);
        }
        else if (id == MainQuestController.SCIENTIST_QUEST_ID)
        {
            ScientistQuestAdvanced(step);
        }
    }

    private void PartnerQuestAdvanced(int step)
    {
        LabelPartner.Text = $"##QUEST_PARTNER_{step.ToString("000")}##";
        PartnerControl.Visible = !PartnerFinished;
    }

    private void ManagerQuestAdvanced(int step)
    {
        LabelManager.Text = $"##QUEST_MANAGER_{step.ToString("000")}##";
        ManagerControl.Visible = !ManagerFinished;
    }

    private void ScientistQuestAdvanced(int step)
    {
        LabelScientist.Text = $"##QUEST_SCIENTIST_{step.ToString("000")}##";
        ScientistControl.Visible = !ScientistFinished;
    }
}
