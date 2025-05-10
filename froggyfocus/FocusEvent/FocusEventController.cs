using System;

public partial class FocusEventController : ResourceController<FocusEventCollection, FocusEventInfo>
{
    public override string Directory => "FocusEvent";
    public static FocusEventController Instance => Singleton.Get<FocusEventController>();

    public event Action OnFocusEventStarted;
    public event Action<FocusEventCompletedResult> OnFocusEventCompleted;
    public event Action<FocusEventFailedResult> OnFocusEventFailed;

    public void FocusEventStarted(FocusEvent e)
    {
        OnFocusEventStarted?.Invoke();
    }

    public void FocusEventCompleted(FocusEventCompletedResult result)
    {
        CurrencyController.Instance.AddValue(CurrencyType.Money, result.Character.CurrencyReward);

        OnFocusEventCompleted?.Invoke(result);

        Data.Game.Save();
    }

    public void FocusEventFailed(FocusEventFailedResult result)
    {
        OnFocusEventFailed?.Invoke(result);
    }
}

public class FocusEventResult
{
    public FocusEvent FocusEvent { get; set; }
    public FocusCharacterInfo Character { get; set; }

    public FocusEventResult(FocusEvent e)
    {
        FocusEvent = e;
        Character = e.Target.Info;
    }
}

public class FocusEventCompletedResult : FocusEventResult
{
    public FocusEventCompletedResult(FocusEvent e) : base(e) { }
}

public class FocusEventFailedResult : FocusEventResult
{
    public FocusEventFailedResult(FocusEvent e) : base(e) { }
}