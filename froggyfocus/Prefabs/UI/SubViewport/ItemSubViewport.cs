public partial class ItemSubViewport : PreviewSubViewport
{
    public void SetHat(AppearanceHatInfo info)
    {
        SetPrefab(info.Prefab);
    }

    public void SetCharacter(FocusCharacterInfo info)
    {
        var character = info.Scene.Instantiate<FocusCharacter>();
        character.Initialize(info);
        SetPreview(character);
    }
}
