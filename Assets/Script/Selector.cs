using System.Collections.Generic;

public class Selector : Node
{
    protected List<Node> nodes = new();

    public Selector(List<Node> nodes) => this.nodes = nodes;

    public override State Evaluate()
    {
        foreach (Node node in nodes)
        {
            switch (node.Evaluate())
            {
                case State.Success:
                    state = State.Success;
                    return state;
                case State.Running:
                    state = State.Running;
                    return state;
            }
        }
        state = State.Failure;
        return state;
    }
}
