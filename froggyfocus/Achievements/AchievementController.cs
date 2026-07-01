using System.Linq;

public partial class AchievementController : SingletonController
{
    public static AchievementController Instance => Singleton.Get<AchievementController>();
    public override string Directory => "Achievements";

    protected override void Initialize()
    {
        base.Initialize();
        GameProfileController.Instance.OnGameProfileSelected += GameProfile_Selected;
        MainQuestController.Instance.OnAnyQuestAdvanced += MainQuest_Advanced;
    }

    private void GameProfile_Selected(int profile)
    {
        UpdateAchievements();
    }

    private void MainQuest_Advanced()
    {
        UpdateAchievements();
    }

    public void UpdateAchievements()
    {
        UpdateItemAchievements();
        UpdateQuestAchievements();
        UpdateGameCompletedAchievement();
    }

    private void UpdateItemAchievements()
    {
        var items = ItemController.Instance.Collection.Resources.Where(x => x.HasAchievement && Item.IsOwned(x.Type));
        foreach (var item in items)
        {
            SetAchievement(item.AchievementId);
        }
    }

    private void UpdateQuestAchievements()
    {
        UpdatePartnerQuestAchievement();
        UpdateManagerQuestAchievement();
        UpdateScientistQuestAchievement();
    }

    private void UpdatePartnerQuestAchievement()
    {
        if (MainQuestController.Instance.GetPartnerStep() >= 5)
        {
            SetAchievement(Achievement.QuestPartner);
        }
    }

    private void UpdateManagerQuestAchievement()
    {
        if (MainQuestController.Instance.GetManagerStep() >= 5)
        {
            SetAchievement(Achievement.QuestManager);
        }
    }

    private void UpdateScientistQuestAchievement()
    {
        if (MainQuestController.Instance.GetScientistStep() >= 4)
        {
            SetAchievement(Achievement.QuestScientist);
        }
    }

    private void UpdateLocationAchievements()
    {
        UpdateEldritchLocationAchievement();
        UpdateCrystalLocationAchievement();
        UpdateGlitchLocationAchievement();
    }

    private void UpdateEldritchLocationAchievement()
    {
        if (Data.Game.EldritchLocationEntered)
        {
            SetAchievement(Achievement.LocationEldritch);
        }
    }

    private void UpdateCrystalLocationAchievement()
    {
        if (Data.Game.CrystalLocationEntered)
        {
            SetAchievement(Achievement.LocationCrystal);
        }
    }

    private void UpdateGlitchLocationAchievement()
    {
        if (Data.Game.GlitchLocationEntered)
        {
            SetAchievement(Achievement.LocationGlitch);
        }
    }

    private void UpdateGameCompletedAchievement()
    {
        if (Data.Game.GameCompleted)
        {
            SetAchievement(Achievement.GameComplete);
        }
    }

    private void SetAchievement(string id)
    {
        if (ApplicationInfo.Instance.Type == ApplicationType.Release)
        {
            SteamController.Instance.SetAchievement(id);
        }
        else
        {
            Debug.LogMethod(id);
        }
    }
}
