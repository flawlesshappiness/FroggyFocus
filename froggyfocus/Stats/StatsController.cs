public partial class StatsController : SingletonController
{
    public override string Directory => "Stats";
    public static StatsController Instance => Singleton.Get<StatsController>();

    public override void _Ready()
    {
        base._Ready();
        RegisterDebugActions();
    }

    private void RegisterDebugActions()
    {
    }
}
