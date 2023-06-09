using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AI : MonoBehaviour
{
    enum State
    {
        Patrolling,
        Chasing,
        Traveling,
        Attacking
    }

    State currentState;
    NavMeshAgent agent;

    public Transform[] destinationPoints;
    int destinationIndex = 6;    

    public Transform player;

    [SerializeField]float visionRange;    

    [SerializeField][Range(0, 360)]float visionAngle;         

    [SerializeField]float patrolRange = 10f;
  
    [SerializeField]Transform patrolZone;    

    [SerializeField]private float timer = 5f;



    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    // Start is called before the first frame update
    void Start()
    {
        currentState = State.Patrolling;

        destinationIndex = Random.Range(0, destinationPoints.Length);
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentState)
        {
            case State.Patrolling:
                Patrol();
            break;
            case State.Chasing:
                Chase();
            break;
            case State.Traveling:
                Travel();
            break;
            case State.Attacking:
                Attack();
            break;
            default:
                Chase();
            break;
        }
    }

    void Patrol()
    {
        agent.destination = destinationPoints[destinationIndex].position;

        if(Vector3.Distance(transform.position, destinationPoints[destinationIndex].position) < 1)
        {
            destinationIndex = Random.Range(0, destinationPoints.Length);
        }

        if(Vector3.Distance(transform.position, player.position) < visionRange)
        {
            currentState = State.Chasing;
        }        
    }

    bool RandomPoint(Vector3 center, float range, out Vector3 point)
    {
        Vector3 randomPoint = center + Random.insideUnitSphere * range;
        NavMeshHit hit;
        if(NavMesh.SamplePosition(randomPoint, out hit, 4, NavMesh.AllAreas))
        {
            point = hit.position;
            return true;
        }
        point = Vector3.zero;
        return false;
    }

    void Travel()
    {
        if(agent.remainingDistance <= 0.2)
        {
            currentState = State.Patrolling;
        }

        if(FindTarget())
        {
            currentState = State.Chasing;
        }
    }

    void Chase()
    {
        agent.destination = player.position;

        if(!FindTarget())
        {
            currentState = State.Patrolling;
        }
    }

    void Attack()
    {
         if(FindTarget() && agent.remainingDistance == 0)
        {
            Debug.Log("La IA esta atacandote");
            currentState = State.Attacking;
            
        }         
        

        if(!FindTarget())
        {
            currentState = State.Patrolling;
        }
    }


    bool FindTarget()
    {
        if(Vector3.Distance(transform.position, player.position) < visionRange)
        {
            Vector3 directionToTarget = (player.position - transform.position).normalized;
            if(Vector3.Angle(transform.forward, directionToTarget) < visionAngle / 2)
            {
                float distanceToTarget = Vector3.Distance(transform.position, player.position);
                if(!Physics.Raycast(transform.position, directionToTarget, distanceToTarget))
                {
                    return true;
                }
            }
        }

        return false;
    }

    void OnDrawGizmos()
    {
        foreach (Transform point in destinationPoints)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(point.position, 1);
        }

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, visionRange);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(patrolZone.position, patrolRange);

    }

}
