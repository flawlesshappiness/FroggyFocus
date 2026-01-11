public partial class TimeController : SingletonController
{
    public static TimeController Instance => Singleton.Get<TimeController>();
    public override string Directory => $"Time";

    private bool game_started;
    private float time_game_started;

    private SaveData current_save_data;

    protected override void Initialize()
    {
        base.Initialize();
        MainMenuView.Instance.OnMainMenuEnter += MainMenuEnter;
        MainMenuView.Instance.OnGameStart += GameStart;

        ProfileSelected(Data.Options.Profile ?? 1);
        GameProfileController.Instance.OnGameProfileSelected += ProfileSelected;
    }

    private void ProfileSelected(int profile)
    {
        if (current_save_data != null)
        {
            current_save_data.OnBeforeSave -= BeforeSave;
        }

        current_save_data = Data.Game;
        current_save_data.OnBeforeSave += BeforeSave;
    }

    private void BeforeSave()
    {
        if (!game_started) return;

        var time_in_seconds = (int)(GameTime.Time - time_game_started);
        Data.Game.GameTime = Data.Game.GameTime.Add(new System.TimeSpan(0, 0, time_in_seconds));

        time_game_started = GameTime.Time;
    }

    private void MainMenuEnter()
    {
        game_started = false;
    }

    private void GameStart()
    {
        game_started = true;
        time_game_started = GameTime.Time;
    }
}
