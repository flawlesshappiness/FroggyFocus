using System.Linq;

public partial class AchievementController : SingletonController
{
    public static AchievementController Instance => Singleton.Get<AchievementController>();
    public override string Directory => "Achievements";

    protected override void Initialize()
    {
        base.Initialize();
        GameProfileController.Instance.OnGameProfileSelected += GameProfile_Selected;
    }

    private void GameProfile_Selected(int profile)
    {
        UpdateAchievements();
    }

    public void UpdateAchievements()
    {
        UpdateItemAchievements();
    }

    private void UpdateItemAchievements()
    {
        var items = ItemController.Instance.Collection.Resources.Where(x => x.HasAchievement && Item.IsOwned(x.Type));
        foreach (var item in items)
        {
            SetAchievement(item.AchievementId);
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
