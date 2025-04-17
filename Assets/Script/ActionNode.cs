using System;

public class ActionNode : Node
{
    private Func<State> action;

    public ActionNode(Func<State> action)
    {
        this.action = action;
    }

    public override State Evaluate()
    {
        return action();
    }
}
