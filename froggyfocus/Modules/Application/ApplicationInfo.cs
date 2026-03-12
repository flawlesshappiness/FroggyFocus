using Godot;

[GlobalClass]
public partial class ApplicationInfo : Resource
{
    private static ApplicationInfo _instance;
    public static ApplicationInfo Instance => _instance ?? (_instance = GD.Load<ApplicationInfo>($"{Paths.Modules}/Application/{nameof(ApplicationInfo)}.tres"));

    [Export]
    public string Version { get; set; }

    [Export]
    public ApplicationType Type { get; set; }

    [Export]
    public string StartScene;

    public string GetVersionString()
    {
        var prefix = Type switch
        {
            ApplicationType.Demo => "Demo",
            ApplicationType.Release => "Release",
            _ => "Internal"
        };
        return $"{prefix} v{Version}";
    }
}
