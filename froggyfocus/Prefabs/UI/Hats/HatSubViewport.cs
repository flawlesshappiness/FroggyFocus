public partial class HatSubViewport : ObjectPreview
{
    public void SetHat(AppearanceHatInfo info)
    {
        SetPrefab(info.Prefab);
    }
}
