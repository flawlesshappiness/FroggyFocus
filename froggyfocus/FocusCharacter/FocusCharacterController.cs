using System.Linq;

public partial class FocusCharacterController : ResourceController<FocusCharacterCollection, FocusCharacterInfo>
{
    public static FocusCharacterController Instance => Singleton.Get<FocusCharacterController>();
    public override string Directory => "FocusCharacter";

    public FocusCharacterInfo GetInfoFromPath(string resource_path)
    {
        return Collection.Resources.FirstOrDefault(x => x.ResourcePath == resource_path);
    }
}
