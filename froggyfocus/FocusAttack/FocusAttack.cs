using Godot;
using System;
using System.Collections;

namespace FlawLizArt.FocusEvent;

public abstract partial class FocusAttack : Node3D
{
    [Export]
    public FocusAttackType Type;

    protected FocusTarget Target;
    protected FocusCursor Cursor => Target.FocusEvent.Cursor;
    protected bool IsActive { get; private set; }

    protected RandomNumberGenerator rng = new();

    public void Initialize(FocusTarget target)
    {
        Target = target;
        target.OnStarted += Target_Started;
        target.OnStopped += Target_Stopped;
        target.OnCursorEnter += Target_CursorEnter;
        target.OnCursorExit += Target_CursorExit;
        target.OnCaught += Target_Caught;

        IsActive = Target.Info.Attacks.Contains(Type);
    }

    private void Target_Started()
    {
        if (IsActive) Started();
    }


    private void Target_Stopped()
    {
        if (IsActive) Stopped();
    }


    private void Target_Caught()
    {
        if (IsActive) Caught();
    }


    private void Target_CursorEnter()
    {
        if (IsActive) CursorEnter();
    }

    private void Target_CursorExit()
    {
        if (IsActive) CursorExit();
    }

    protected virtual void Started() { }
    protected virtual void Stopped() { }
    protected virtual void Caught() { }
    protected virtual void CursorEnter() { }
    protected virtual void CursorExit() { }

    protected void StartState()
    {
        Target.SetState(FocusTarget.State.Attack);
    }

    protected void EndState()
    {
        Target.SetState(FocusTarget.State.Moving);
    }

    protected Coroutine WaitFor(float duration, Action action) => WaitFor(duration, "wait_for", action);
    protected Coroutine WaitFor(float duration, string id, Action action)
    {
        return this.StartCoroutine(Cr, id);
        IEnumerator Cr()
        {
            yield return new WaitForSeconds(duration);
            action?.Invoke();
        }
    }

    protected void StopCursorFocus()
    {
        Cursor.EndFocusTarget();
    }

    protected void HurtFocusValue(float perc)
    {
        Target.HurtFocusValue(perc);
    }
}
