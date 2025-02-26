using System;

namespace SkillQuest.Core;

public struct DisposeAction(Action action) : IDisposable
{
    public Action Action = action;

    public void Dispose()
    {
        Action action = this.Action;
        if (action != null)
            action();
        this.Action = (Action) null;
    }

    public static IDisposable Create(Action action) => (IDisposable) new DisposeAction(action);
}
