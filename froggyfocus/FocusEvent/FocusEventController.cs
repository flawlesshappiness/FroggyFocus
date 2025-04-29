public partial class FocusEventController : ResourceController<FocusEventCollection, FocusEventInfo>
{
    public static FocusEventController Instance => Singleton.Get<FocusEventController>();
    public override string Directory => "FocusEvent";
}
