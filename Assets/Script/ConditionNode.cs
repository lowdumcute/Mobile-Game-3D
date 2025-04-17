using System;

public class ConditionNode : Node
{
    private Func<bool> condition;

    public ConditionNode(Func<bool> condition)
    {
        this.condition = condition;
    }

    public override State Evaluate()
    {
        return condition() ? State.Success : State.Failure;
    }
}
