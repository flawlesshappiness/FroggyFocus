public partial class HandInRequestController : ResourceController<HandInRequestInfoCollection, HandInRequestInfo>
{
    public static HandInRequestController Instance => Singleton.Get<HandInRequestController>();
    public override string Directory => "HandInRequest";
}
