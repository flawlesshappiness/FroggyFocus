using System.Collections;

public static class FocusTargetAction
{
    public static IEnumerator WaitForAction(FocusTargetActionType type, FocusTarget target)
    {
        yield return type switch
        {
            FocusTargetActionType.Stun => Stun(type, target),
            _ => null,
        };
    }

    public static IEnumerator Stun(FocusTargetActionType type, FocusTarget target)
    {
        target.ExclamationMark.AnimateShow();

        var completed = false;
        var time_end = GameTime.Time + 1f;
        while (GameTime.Time < time_end)
        {
            if (PlayerInput.Interact.Pressed)
            {
                target.Stun(2f);
                completed = true;
                break;
            }

            yield return null;
        }

        if (completed)
        {
            target.ExclamationMark.AnimateBounce();
        }
        else
        {
            target.ExclamationMark.AnimateHide();
        }
    }
}
