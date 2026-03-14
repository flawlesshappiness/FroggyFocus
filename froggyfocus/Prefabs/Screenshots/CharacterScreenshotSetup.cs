public partial class CharacterScreenshotSetup : ScreenshotSceneSetup
{
    protected override void OnShowSetup()
    {
        base.OnShowSetup();
        InitializeFrogs();
        InitializeBugs();
    }

    private void InitializeFrogs()
    {
        var frogs = this.GetNodesInChildren<FrogCharacter>();
        foreach (var frog in frogs)
        {
            frog.LoadAppearance();
        }
    }

    private void InitializeBugs()
    {
        var bugs = this.GetNodesInChildren<FocusCharacter>();
        foreach (var bug in bugs)
        {
            bug.SetAccessory(null);
        }
    }
}
