using System;

public partial class MainQuestController : SingletonController
{
    public static MainQuestController Instance => Singleton.Get<MainQuestController>();
    public override string Directory => "MainQuest";

    public const string PARTNER_QUEST_ID = "PARTNER_QUEST";
    public const string MANAGER_QUEST_ID = "MANAGER_QUEST";
    public const string SCIENTIST_QUEST_ID = "SCIENTIST_QUEST";

    public event Action OnAnyQuestAdvanced;

    protected override void Initialize()
    {
        base.Initialize();
        InventoryController.Instance.OnCharacterAdded += InventoryCharacterAdded;
        GameFlagsController.Instance.OnFlagChanged += FlagChanged;
    }

    private void InventoryCharacterAdded(InventoryCharacterData data)
    {
        var info = FocusCharacterController.Instance.GetInfoFromPath(data.InfoPath);
        if (info == FocusCharacterController.Instance.Collection.BugOfLove)
        {
            AdvancePartnerQuest(4);
        }
        else if (info == FocusCharacterController.Instance.Collection.BugOfData)
        {
            AdvanceManagerQuest(4);
        }
        else if (info == FocusCharacterController.Instance.Collection.BugOfLife)
        {
            AdvanceScientistQuest(3);
        }
    }

    private void FlagChanged(string id, int step)
    {
        if (id == PARTNER_QUEST_ID || id == MANAGER_QUEST_ID || id == SCIENTIST_QUEST_ID)
        {
            OnAnyQuestAdvanced?.Invoke();
        }
    }

    public void AdvancePartnerQuest(int step)
    {
        AdvanceQuest(PARTNER_QUEST_ID, step);
    }

    public void AdvanceManagerQuest(int step)
    {
        AdvanceQuest(MANAGER_QUEST_ID, step);
    }

    public void AdvanceScientistQuest(int step)
    {
        AdvanceQuest(SCIENTIST_QUEST_ID, step);
    }

    private bool AdvanceQuest(string id, int step)
    {
        Debug.TraceMethod($"{id}, {step}");

        var current = GameFlags.GetFlag(id);
        if (current != step - 1)
        {
            Debug.Trace($"Failed to advance quest to {step} because GameFlag is {current}");
            return false;
        }

        GameFlags.SetFlag(id, step);
        Data.Game.Save();

        return true;
    }

    public int GetPartnerStep() => GameFlags.GetFlag(PARTNER_QUEST_ID);
    public int GetManagerStep() => GameFlags.GetFlag(MANAGER_QUEST_ID);
    public int GetScientistStep() => GameFlags.GetFlag(SCIENTIST_QUEST_ID);
}
