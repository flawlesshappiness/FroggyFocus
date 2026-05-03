using Godot;
using System.Collections;
using System.Collections.Generic;

public partial class FocusIntroCamera : Node3D
{
    [Export]
    public Camera3D Camera;

    [Export]
    public Marker3D CameraMarker;

    [Export]
    public GpuParticles3D ps_clouds_in;

    [Export]
    public GpuParticles3D ps_clouds_out;

    private List<ParticleProcessMaterial> ps_process_materials = new();

    private class Settings
    {
        public Transform3D Start { get; set; }
        public Transform3D End { get; set; }
        public GpuParticles3D Particles { get; set; }
    }

    public override void _Ready()
    {
        base._Ready();
        InitializeParticles();
    }

    private void InitializeParticles()
    {
        ps_process_materials.Clear();
        var particles = new List<GpuParticles3D> { ps_clouds_in, ps_clouds_out };
        foreach (var ps in particles)
        {
            var mat = ps.ProcessMaterial.Duplicate() as ParticleProcessMaterial;
            ps.ProcessMaterial = mat;
            ps_process_materials.Add(mat);
        }
    }

    public void SetColor(Color color)
    {
        foreach (var mat in ps_process_materials)
        {
            mat.Color = color;
        }
    }

    public Coroutine AnimateUpToFocusEvent()
    {
        CameraMarker.GlobalPosition = Player.Instance.Camera.GlobalPosition.Add(y: 6f);
        CameraMarker.GlobalRotation = CameraMarker.GlobalRotation.Set(y: Player.Instance.Camera.GlobalRotation.Y);
        return AnimateCamera(new Settings
        {
            Start = Player.Instance.Camera.GlobalTransform,
            End = CameraMarker.GlobalTransform,
            Particles = ps_clouds_in
        });
    }

    public Coroutine AnimateDownToFocusEvent(Camera3D camera_end)
    {
        CameraMarker.GlobalPosition = camera_end.GlobalPosition.Add(y: 1f);
        CameraMarker.GlobalRotation = CameraMarker.GlobalRotation.Set(y: camera_end.GlobalRotation.Y);
        return AnimateCamera(new Settings
        {
            Start = CameraMarker.GlobalTransform,
            End = camera_end.GlobalTransform,
            Particles = ps_clouds_out
        });
    }

    public Coroutine AnimateUpFromFocusEvent(Camera3D camera_start)
    {
        CameraMarker.GlobalPosition = camera_start.GlobalPosition.Add(y: 1f);
        CameraMarker.GlobalRotation = CameraMarker.GlobalRotation.Set(y: camera_start.GlobalRotation.Y);
        return AnimateCamera(new Settings
        {
            Start = camera_start.GlobalTransform,
            End = CameraMarker.GlobalTransform,
            Particles = ps_clouds_in
        });
    }

    public Coroutine AnimateDownFromFocusEvent()
    {
        CameraMarker.GlobalPosition = Player.Instance.Camera.GlobalPosition.Add(y: 6f);
        CameraMarker.GlobalRotation = CameraMarker.GlobalRotation.Set(y: Player.Instance.Camera.GlobalRotation.Y);
        return AnimateCamera(new Settings
        {
            Start = CameraMarker.GlobalTransform,
            End = Player.Instance.Camera.GlobalTransform,
            Particles = ps_clouds_out
        });
    }

    private Coroutine AnimateCamera(Settings settings)
    {
        return this.StartCoroutine(Cr, nameof(AnimateCamera));
        IEnumerator Cr()
        {

            var curve = Curves.EaseInOutQuad;
            Camera.GlobalTransform = settings.Start;
            Camera.Current = true;

            settings.Particles.Emitting = true;
            yield return LerpEnumerator.Lerp01(1f, f =>
            {
                var t = curve.Evaluate(f);
                Camera.GlobalTransform = settings.Start.InterpolateWith(settings.End, t);
            });
            settings.Particles.Emitting = false;
        }
    }
}
