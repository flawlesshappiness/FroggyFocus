public partial class InventorySubViewport : PreviewSubViewport
{
    public void SetCharacter(FocusCharacterInfo info)
    {
        var character = info.Scene.Instantiate<FocusCharacter>();
        character.Initialize(info);
        SetPreview(character);
    }
}
