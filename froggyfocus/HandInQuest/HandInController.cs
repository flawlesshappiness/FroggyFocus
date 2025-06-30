using System;
using System.Linq;

public partial class HandInController : ResourceController<HandInCollection, HandInInfo>
{
    public override string Directory => "HandInQuest";
    public static HandInController Instance => Singleton.Get<HandInController>();

    public event Action<string> OnHandInClaimed;

    public HandInInfo GetInfo(string id)
    {
        return Collection.Resources.FirstOrDefault(x => x.Id == id);
    }

    public void HandInClaimed(HandInData data)
    {
        OnHandInClaimed?.Invoke(data.Id);
    }
}
