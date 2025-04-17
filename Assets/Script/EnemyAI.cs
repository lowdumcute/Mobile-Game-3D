using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class EnemyAI : MonoBehaviour
{
    public Transform player;
    public float chaseRange = 10f;
    public float attackRange = 2f;

    private Node topNode;
    private NavMeshAgent agent;
    private Animator anim;

    private Vector3 patrolTarget;
    private float patrolRange = 5f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();

        topNode = BuildBehaviorTree();
    }

    void Update()
    {
        topNode.Evaluate();
    }

    private Node BuildBehaviorTree()
    {
        Node isInAttackRange = new ConditionNode(() =>
            Vector3.Distance(transform.position, player.position) <= attackRange);

        Node attack = new ActionNode(() =>
        {
            agent.SetDestination(transform.position); // Dừng lại
            anim.SetTrigger("Attack");
            anim.SetFloat("MoveSpeed", 0f); // Idle
            return Node.State.Success;
        });

        Node attackSequence = new Sequence(new List<Node> { isInAttackRange, attack });

        Node isInChaseRange = new ConditionNode(() =>
            Vector3.Distance(transform.position, player.position) <= chaseRange);

        Node chase = new ActionNode(() =>
        {
            agent.SetDestination(player.position);
            anim.SetFloat("MoveSpeed", 1f); // Run
            return Node.State.Running;
        });

        Node chaseSequence = new Sequence(new List<Node> { isInChaseRange, chase });

        Node patrol = new ActionNode(() =>
        {
            if (!agent.hasPath || agent.remainingDistance < 0.5f)
            {
                for (int i = 0; i < 10; i++) // thử tối đa 10 lần
                {
                    Vector3 randomDirection = Random.insideUnitSphere * patrolRange;
                    randomDirection.y = 0; // giữ cùng mặt phẳng
                    randomDirection += transform.position;

                    if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, patrolRange, NavMesh.AllAreas))
                    {
                        patrolTarget = hit.position;
                        agent.SetDestination(patrolTarget);
                        break;
                    }
                }
            }

            anim.SetFloat("MoveSpeed", 0.5f); // Walk
            return Node.State.Running;
        });


        return new Selector(new List<Node> { attackSequence, chaseSequence, patrol });
    }
}
