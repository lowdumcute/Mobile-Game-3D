using System.Collections.Generic;

public class Sequence : Node
{
    protected List<Node> nodes = new();

    public Sequence(List<Node> nodes) => this.nodes = nodes;

    public override State Evaluate()
    {
        foreach (Node node in nodes)
        {
            switch (node.Evaluate())
            {
                case State.Failure:
                    state = State.Failure;
                    return state;
                case State.Running:
                    state = State.Running;
                    return state;
            }
        }
        state = State.Success;
        return state;
    }
}

