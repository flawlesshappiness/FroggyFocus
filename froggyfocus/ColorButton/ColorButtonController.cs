using System;

public partial class ColorButtonController : SingletonController
{
    public static ColorButtonController Instance => Singleton.Get<ColorButtonController>();
    public override string Directory => "ColorButton";

    public event Action<ColorButtonType> OnTypeChanged;

    private ColorButtonType current_type;

    public void ChangeType(ColorButtonType type)
    {
        current_type = type;
        OnTypeChanged?.Invoke(type);
    }
}
