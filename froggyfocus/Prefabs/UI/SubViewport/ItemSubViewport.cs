public partial class ItemSubViewport : PreviewSubViewport
{
    public void SetHat(AppearanceHatInfo info)
    {
        SetPrefab(info.Prefab);
    }
}
