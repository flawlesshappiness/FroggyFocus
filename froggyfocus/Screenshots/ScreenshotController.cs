using Godot;
using System.Collections;

public partial class ScreenshotController : SingletonController
{
    public override string Directory => "Screenshots";

    private bool prepared_screenshot;
    private string prepared_filepath;

    public override void _Ready()
    {
        base._Ready();
        RegisterDebugActions();
    }

    private void RegisterDebugActions()
    {
        var category = "SCREENSHOTS";

        Debug.RegisterAction(new DebugAction
        {
            Id = category,
            Category = category,
            Text = "Take screenshots",
            Action = DebugTakeScreenshots
        });

        void DebugTakeScreenshots(DebugView view)
        {
            view.PopupStringInput("Filename", s =>
            {
                view.Close();
                TakeScreenshots($"res://Screenshots/Images/{s}");
            });
        }

        Debug.RegisterAction(new DebugAction
        {
            Id = category,
            Category = category,
            Text = "Prepare screenshots",
            Action = DebugPrepareScreenshots
        });

        void DebugPrepareScreenshots(DebugView v)
        {
            v.PopupStringInput("Filename", s =>
            {
                v.Close();
                prepared_screenshot = true;
                prepared_filepath = $"res://Screenshots/Images/{s}";
            });
        }
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);

        if (prepared_screenshot && PlayerInput.DebugScreenshot.Pressed)
        {
            prepared_screenshot = false;
            TakeScreenshots(prepared_filepath);
        }
    }

    public void TakeScreenshots(string image_file_path)
    {
        this.StartCoroutine(Cr, nameof(SaveImage))
            .SetRunWhilePaused();

        IEnumerator Cr()
        {
            var current_size = Scene.Root.Size;

            Scene.PauseLock.AddLock(nameof(ScreenshotController));

            var file_path_no_ext = image_file_path.RemoveExtension();
            //yield return SaveImageByResolution(new Vector2I(3840, 1240), file_path_no_ext);
            yield return SaveImageByResolution(new Vector2I(1920, 1080), file_path_no_ext);
            //yield return SaveImageByResolution(new Vector2I(1280, 720), file_path_no_ext);
            //yield return SaveImageByResolution(new Vector2I(462, 462), file_path_no_ext);

            // Reset resolution
            Scene.Root.Size = current_size;
            Scene.PauseLock.RemoveLock(nameof(ScreenshotController));
        }
    }

    private IEnumerator SaveImageByResolution(Vector2I resolution, string file_path_no_ext)
    {
        Scene.Root.Size = resolution;
        yield return new WaitForSecondsUnscaled(0.5f);
        SaveImage($"{file_path_no_ext}_{resolution.X}x{resolution.Y}.png");
        yield return null;
    }

    private void SaveImage(string filename)
    {
        GetViewport().GetTexture().GetImage().SavePng(filename);
    }
}
