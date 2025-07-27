using Godot;
using System;
using System.Collections;
using System.Collections.Generic;

public partial class MusicController : SingletonController
{
    public static MusicController Instance => Singleton.Get<MusicController>();
    public override string Directory => "Music";

    public readonly MultiLock MuteLock = new MultiLock();

    private Coroutine cr_music;
    private bool skip;
    private AudioStreamPlayer current_asp;
    private bool music_playing;

    public List<string> bgms = new()
    {
        "bgm_jolly_knight"
    };

    public override void _Ready()
    {
        base._Ready();
        MuteLock.OnFree += Mute_Free;
        MuteLock.OnLocked += Mute_Locked;
        RegisterDebugActions();
    }

    private void RegisterDebugActions()
    {
        var category = "MUSIC";

        Debug.RegisterAction(new DebugAction
        {
            Category = category,
            Text = "Skip",
            Action = v => { v.Close(); Skip(); }
        });
    }

    public void StartMusic()
    {
        Coroutine.Stop(cr_music);
        cr_music = this.StartCoroutine(Cr);
        IEnumerator Cr()
        {
            var rng = new RandomNumberGenerator();
            while (true)
            {
                yield return WaitForSkippableDelay(rng.RandfRange(400, 600));

                music_playing = true;
                var bgm = bgms.Random();
                current_asp = SoundController.Instance.Play(bgm);
                current_asp.ProcessMode = ProcessModeEnum.Always;
                var length = current_asp.Stream.GetLength();

                yield return WaitForSkippableDelay(Convert.ToSingle(length));

                music_playing = false;
                current_asp.Stop();
                current_asp = null;
            }
        }

        IEnumerator WaitForSkippableDelay(float delay)
        {
            var start = GameTime.Time;
            var end = start + delay;
            while (GameTime.Time < end)
            {
                if (skip)
                {
                    skip = false;
                    break;
                }

                yield return null;
            }
        }
    }

    public void Skip()
    {
        skip = true;
    }

    private void Mute_Free()
    {
        if (current_asp != null)
        {
            current_asp.FadeIn(0.5f, 0);
        }
    }

    private void Mute_Locked()
    {
        if (current_asp != null)
        {
            current_asp.FadeOut(0.25f);
        }
    }

    private void SetMuted(bool muted)
    {
        var bus = AudioBus.Get("BGM");
        bus.SetMuted(muted);
    }

    public bool IsMusicPlaying()
    {
        return music_playing;
    }
}
