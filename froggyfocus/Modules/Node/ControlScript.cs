using Godot;
using System;
using System.Collections;

public partial class ControlScript : Control
{
    private bool _initialized;
    private bool _visible;

    public Coroutine StartCoroutine(Func<IEnumerator> enumerator, string id = null)
    {
        id = (id ?? string.Empty) + GetInstanceId();
        return Coroutine.Start(enumerator, id, this);
    }

    protected virtual void Initialize()
    {
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        Process_Initialize();
        Process_Visible();
    }

    private void Process_Visible()
    {
        var visible = IsVisibleInTree();
        if (visible != _visible)
        {
            _visible = visible;
            if (visible)
            {
                OnShow();
            }
            else
            {
                OnHide();
            }
        }
    }

    private void Process_Initialize()
    {
        if (!_initialized)
        {
            _initialized = true;
            Initialize();
        }
    }

    protected virtual void OnShow()
    {

    }

    protected virtual void OnHide()
    {

    }
}
