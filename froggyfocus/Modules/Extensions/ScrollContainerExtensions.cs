using Godot;

public static class ScrollContainerExtensions
{
    public static void ScrollVerticalToBottom(this ScrollContainer container) => container.ScrollVerticalDeferred(int.MaxValue);
    public static void ScrollVerticalToTop(this ScrollContainer container) => container.ScrollVerticalDeferred(0);
    public static void ScrollHorizontalToBottom(this ScrollContainer container) => container.ScrollHorizontalDeferred(int.MaxValue);
    public static void ScrollHorizontalToTop(this ScrollContainer container) => container.ScrollHorizontalDeferred(0);
    public static void ScrollVerticalDeferred(this ScrollContainer container, int value) => container.SetDeferred("scroll_vertical", value);
    public static void ScrollHorizontalDeferred(this ScrollContainer container, int value) => container.SetDeferred("scroll_horizontal", value);
}
