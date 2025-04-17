public abstract class Node
{
    public enum State { Running, Success, Failure }
    protected State state;

    public State NodeState => state;

    public abstract State Evaluate();
}
