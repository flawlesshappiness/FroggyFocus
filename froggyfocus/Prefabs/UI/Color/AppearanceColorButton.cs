using Godot;

public partial class AppearanceColorButton : ButtonScript
{
    [Export]
    public TextureRect TextureRect;

    [Export]
    public ItemSubViewport ItemSubViewport;

    [Export]
    public PackedScene PaintBucketPrefab;

    private bool initialized;
    private PaintBucket paint_bucket;

    public override void _Ready()
    {
        base._Ready();
        TextureRect.Texture = ItemSubViewport.GetTexture();
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        if (!initialized)
        {
            initialized = true;
            Initialize();
        }
    }

    private void Initialize()
    {
        InitializePaintBucket();
    }

    private void InitializePaintBucket()
    {
        if (paint_bucket != null) return;
        paint_bucket = PaintBucketPrefab.Instantiate<PaintBucket>();
        ItemSubViewport.SetPreview(paint_bucket);
    }

    public void SetColor(AppearanceColorInfo info)
    {
        InitializePaintBucket();
        paint_bucket.SetPaintColor(info.Color);
    }
}
