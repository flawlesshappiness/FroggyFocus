using Godot;
using System.Collections;

public partial class ScreenshotController : SingletonController
{
    public override string Directory => "Screenshots";

    private bool prepared_screenshot;

    private string filename;
    private string filepath;
    private int screenshot_width = 1920;
    private int screenshot_height = 1080;

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
            Text = "Take screenshot",
            Action = DebugTakeScreenshot
        });

        void DebugTakeScreenshot(DebugView view)
        {
            view.PopupStringInput("Filename", null, s =>
            {
                view.Close();
                TakeScreenshots($"res://Screenshots/Images/{s}");
            });
        }

        Debug.RegisterAction(new DebugAction
        {
            Id = category,
            Category = category,
            Text = "Prepare screenshot",
            Action = DebugPrepareScreenshot
        });

        void DebugPrepareScreenshot(DebugView v)
        {
            v.PopupStringInput("Filename", filename, s =>
            {
                v.Close();
                prepared_screenshot = true;
                filename = s;
                filepath = $"res://Screenshots/Images/{s}";
            });
        }

        Debug.RegisterAction(new DebugAction
        {
            Id = category,
            Category = category,
            Text = "Set resolution",
            Action = ListResolutions
        });

        void ListResolutions(DebugView v)
        {
            v.SetContent_Search();

            v.ContentSearch.AddItem($"Current: {screenshot_width}x{screenshot_height}", null);
            v.ContentSearch.AddItem($"3840x1240", () => SetResolution(v, 3840, 1240));
            v.ContentSearch.AddItem($"1920x1080", () => SetResolution(v, 1920, 1080));
            v.ContentSearch.AddItem($"1280x720", () => SetResolution(v, 1280, 720));
            v.ContentSearch.AddItem($"462x462", () => SetResolution(v, 462, 462));
            v.ContentSearch.AddItem($"Custom", () => SetResolutionFromInput(v));

            v.ContentSearch.UpdateButtons();
        }

        void SetResolution(DebugView v, int width, int height)
        {
            screenshot_width = width;
            screenshot_height = height;
            ListResolutions(v);
        }

        void SetResolutionFromInput(DebugView v)
        {
            v.PopupStringInput("Resolution: 0000x0000", $"{screenshot_width}x{screenshot_height}", s =>
            {
                var split = s.Split('x');
                if (split.Length != 2) return;

                screenshot_width = int.TryParse(split[0], out var x) ? x : screenshot_width;
                screenshot_height = int.TryParse(split[1], out var y) ? y : screenshot_height;

                ListResolutions(v);
            });
        }
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);

        if (prepared_screenshot && PlayerInput.DebugScreenshot.Pressed)
        {
            prepared_screenshot = false;
            TakeScreenshots(filepath);
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

            var scale_mode = Scene.Root.ContentScaleMode;
            Scene.Root.ContentScaleMode = Window.ContentScaleModeEnum.Disabled;

            var file_path_no_ext = image_file_path.RemoveExtension();
            yield return SaveImageByResolution(new Vector2I(screenshot_width, screenshot_height), file_path_no_ext);

            Scene.Root.ContentScaleMode = scale_mode;

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
