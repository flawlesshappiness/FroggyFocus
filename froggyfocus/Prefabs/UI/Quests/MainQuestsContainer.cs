using Godot;

public partial class MainQuestsContainer : ControlScript
{
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

    public override void _Ready()
    {
        base._Ready();
        MainQuestController.Instance.OnPartnerQuestAdvanced += PartnerQuestAdvanced;
        MainQuestController.Instance.OnManagerQuestAdvanced += ManagerQuestAdvanced;
        MainQuestController.Instance.OnScientistQuestAdvanced += ScientistQuestAdvanced;
    }

    protected override void Initialize()
    {
        base.Initialize();
        GameProfileController.Instance.OnGameProfileSelected += GameProfileSelected;
        Load();
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

    private void PartnerQuestAdvanced(int step)
    {
        LabelPartner.Text = $"##QUEST_PARTNER_{step.ToString("000")}##";
        PartnerControl.Visible = step < 5;
    }

    private void ManagerQuestAdvanced(int step)
    {
        LabelManager.Text = $"##QUEST_MANAGER_{step.ToString("000")}##";
        ManagerControl.Visible = step < 5;
    }

    private void ScientistQuestAdvanced(int step)
    {
        LabelScientist.Text = $"##QUEST_SCIENTIST_{step.ToString("000")}##";
        ScientistControl.Visible = step < 4;
    }
}
