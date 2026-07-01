using Godot;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public partial class AchievementScreenshotSetup : ScreenshotSceneSetup
{
    [Export]
    public Node3D ItemParent;

    [Export]
    public Node3D QuestPartner;

    [Export]
    public Node3D QuestManager;

    [Export]
    public Node3D QuestScientist;

    [Export]
    public Node3D LocationEldritch;

    [Export]
    public Node3D LocationCrystal;

    [Export]
    public Node3D LocationGlitch;

    [Export]
    public Node3D GameCompleted;

    private const string path_screenshots = $"res://Screenshots/Achievements/";
    private List<ItemMap> items = new();

    private class ItemMap
    {
        public string Name { get; set; }
        public Node3D Node { get; set; }
        public Texture2D Texture { get; set; }
        public Texture2D TextureGreyscale { get; set; }
    }

    protected override void OnShowSetup()
    {
        base.OnShowSetup();
        Run();
    }

    public void Run()
    {
        this.StartCoroutine(Cr, "run")
            .SetRunWhilePaused(true);

        IEnumerator Cr()
        {
            yield return null;
            yield return LoadAppearances();
            yield return LoadOtherAchievements();
            yield return TakeScreenshots();
            yield return GenerateGreyscaleTextures();
            yield return SaveScreenshots();
            yield return PrintNames();
            yield return Cleanup();
        }
    }

    private IEnumerator LoadAppearances()
    {
        Debug.Log("Loading items...");

        var valid_items = ItemController.Instance.Collection.Resources.Where(x => x.HasAchievement);

        foreach (var item_info in valid_items)
        {
            var appearance_info = AppearanceController.Instance.GetInfo(item_info.Type);
            var attachment = LoadItem<AppearanceAttachment>(appearance_info.Prefab);
            attachment.SetDefaultColors();

            var item = new ItemMap
            {
                Name = item_info.AchievementId,
                Node = attachment
            };

            items.Add(item);
        }

        yield return null;
    }

    private IEnumerator LoadOtherAchievements()
    {
        items.Add(new ItemMap
        {
            Name = Achievement.QuestPartner,
            Node = QuestPartner
        });

        items.Add(new ItemMap
        {
            Name = Achievement.QuestManager,
            Node = QuestManager
        });

        items.Add(new ItemMap
        {
            Name = Achievement.QuestScientist,
            Node = QuestScientist
        });

        items.Add(new ItemMap
        {
            Name = Achievement.LocationEldritch,
            Node = LocationEldritch
        });

        items.Add(new ItemMap
        {
            Name = Achievement.LocationCrystal,
            Node = LocationCrystal
        });

        items.Add(new ItemMap
        {
            Name = Achievement.LocationGlitch,
            Node = LocationGlitch
        });

        items.Add(new ItemMap
        {
            Name = Achievement.GameComplete,
            Node = GameCompleted
        });

        QuestPartner.Hide();
        QuestManager.Hide();
        QuestScientist.Hide();
        LocationEldritch.Hide();
        LocationCrystal.Hide();
        LocationGlitch.Hide();
        GameCompleted.Hide();

        yield return null;
    }

    private T LoadItem<T>(PackedScene prefab)
        where T : Node3D
    {
        var node = prefab.Instantiate<T>();
        node.SetParent(ItemParent);
        node.ClearPositionAndRotation();
        node.Hide();
        return node;
    }

    private IEnumerator TakeScreenshots()
    {
        Debug.Log("Taking screenshots...");

        var root_size = Scene.Root.Size;
        var resolution = new Vector2I(256, 256);
        Scene.Root.Size = resolution;
        Scene.Root.ContentScaleSize = resolution;
        yield return null;

        foreach (var item in items)
        {
            item.Node.Show();
            yield return null;
            var img_tex = new ImageTexture();
            img_tex.SetImage(GetViewport().GetTexture().GetImage());
            item.Texture = img_tex;
            item.Node.Hide();
        }

        yield return null;
        Scene.Root.Size = root_size;
        Scene.Root.ContentScaleSize = root_size;
        yield return null;
    }

    private IEnumerator GenerateGreyscaleTextures()
    {
        Debug.Log("Generating greyscale textures...");

        foreach (var item in items)
        {
            item.TextureGreyscale = GenerateGreyscaleTexture(item.Texture);
        }

        yield return null;
    }

    private Texture2D GenerateGreyscaleTexture(Texture2D texture)
    {
        var image = texture.GetImage();
        image.Convert(Image.Format.La8);
        image.Convert(Image.Format.Rgba8);

        var img_tex = new ImageTexture();
        img_tex.SetImage(image);
        return img_tex;
    }

    private IEnumerator SaveScreenshots()
    {
        Debug.Log("Saving screenshots...");

        foreach (var item in items)
        {
            item.Texture.GetImage().SavePng($"{path_screenshots}{item.Name}.png");
            item.TextureGreyscale.GetImage().SavePng($"{path_screenshots}{item.Name}_GREY.png");
        }

        yield return null;
    }

    private IEnumerator PrintNames()
    {
        Debug.Log("Printing names...");

        var ordered = items.OrderBy(x => x.Name);
        ordered.ForEach(x => Debug.Log(x.Name));

        yield return null;
    }

    private IEnumerator Cleanup()
    {
        foreach (var item in items)
        {
            item.Node.QueueFree();
        }

        items.Clear();

        yield return null;
    }
}
