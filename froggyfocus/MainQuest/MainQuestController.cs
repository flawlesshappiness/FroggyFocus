using System;

public partial class MainQuestController : SingletonController
{
    public static MainQuestController Instance => Singleton.Get<MainQuestController>();
    public override string Directory => "MainQuest";

    private readonly string PARTNER_QUEST_ID = "PARTNER_QUEST";
    private readonly string MANAGER_QUEST_ID = "MANAGER_QUEST";
    private readonly string SCIENTIST_QUEST_ID = "SCIENTIST_QUEST";

    public event Action<int> OnPartnerQuestAdvanced;
    public event Action<int> OnManagerQuestAdvanced;
    public event Action<int> OnScientistQuestAdvanced;
    public event Action OnAnyQuestAdvanced;

    public void AdvancePartnerQuest(int step)
    {
        if (AdvanceQuest(PARTNER_QUEST_ID, step))
        {
            OnPartnerQuestAdvanced?.Invoke(step);
        }
    }

    public void AdvanceManagerQuest(int step)
    {
        if (AdvanceQuest(MANAGER_QUEST_ID, step))
        {
            OnManagerQuestAdvanced?.Invoke(step);
        }
    }

    public void AdvanceScientistQuest(int step)
    {
        if (AdvanceQuest(MANAGER_QUEST_ID, step))
        {
            OnScientistQuestAdvanced?.Invoke(step);
        }
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

        OnAnyQuestAdvanced?.Invoke();
        return true;
    }

    public int GetPartnerStep() => GameFlags.GetFlag(PARTNER_QUEST_ID);
    public int GetManagerStep() => GameFlags.GetFlag(MANAGER_QUEST_ID);
    public int GetScientistStep() => GameFlags.GetFlag(SCIENTIST_QUEST_ID);
}
