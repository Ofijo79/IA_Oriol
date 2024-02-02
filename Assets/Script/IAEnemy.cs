using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class IAEnemy : MonoBehaviour
{
    enum State
    {
        Patrolling,
        Chasing,
        Searching,
        Waiting,
        Attacking
    }

    State currentState;

    NavMeshAgent enemyAgent;

    Transform playerTransform;

    [SerializeField] Transform  patrolAreaCenter;
    
    [SerializeField] Vector2 patrolAreaSize;

    [SerializeField] float visionRange = 15;
    [SerializeField] float visionAngle = 90;

    Vector3 lastTargetPosition;

    [SerializeField]float searchTimer;

    [SerializeField]float searchWaitTime = 15;

    [SerializeField]float searchRadius = 30;

    // Start is called before the first frame update
    void Awake()
    {
        enemyAgent = GetComponent<NavMeshAgent>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Start()
    {
        currentState = State.Patrolling;
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

            case State.Searching:
                Search();
            break;
            
            case State.Waiting:
                Waiting();
            break;

            case State.Attacking:
                Attacking();
            break;
        }
    }

    void Patrol()
    {    
        if(enemyAgent.remainingDistance < 0.5f)
        {
            SetRandomPoint();
        }

        if(OnRange() == true)
        {
            currentState = State.Chasing;
        }
    }

    void Chase()
    {
        enemyAgent.destination = playerTransform.position;

        if(OnRange() == false)
        {
            currentState = State.Searching;
        }
    }

    void Search()
    {
        if(OnRange() == true)
        {
            searchTimer = 0;
            currentState = State.Chasing;
        }
        searchTimer += Time.deltaTime;

        if(searchTimer < searchWaitTime)
        {
            if(enemyAgent.remainingDistance < 0.5f)
            {
                Debug.Log("Buscando punto aleatorio");
                Vector3 randomSearchPoint = lastTargetPosition + Random.insideUnitSphere * searchRadius;

                randomSearchPoint.y = lastTargetPosition.y;

                enemyAgent.destination = randomSearchPoint;
            }
        }
        else
        {
            currentState = State.Patrolling;
        }
    }

    void Waiting()
    {

    }

    void Attacking()
    {
        enemyAgent.destination = playerTransform.position;

        Debug.Log("Attacking");
    }

    void SetRandomPoint()
    {
        //float randomX = Random.Range(-patrolAreaSize.x / 2, patrolAreaSize.x / 2);
        
        //float randomZ = Random.Range(-patrolAreaSize.y / 2, patrolAreaSize.y / 2);

        Vector3 punto1 = new Vector3(30.66f, 12.61f, 3.64f);
        Vector3 punto2 = new Vector3(30.07f, 12.61f, 8.16f);
        Vector3 punto3 = new Vector3(35.38f, 12.61f, 3.64f);
        enemyAgent.destination = punto1;

        //Vector3 randomPoint = new Vector3(randomX, 0f, 2.26f) + patrolAreaCenter.position;

        if(enemyAgent.remainingDistance < 0.5f)
        {
            currentState = State.Waiting;
        }

    
    }

    bool OnRange()
    {
        /*if(Vector3.Distance(transform.position, playerTransform.position) <= visionRange)
        {
            return true;
        }

        return false;*/

        Vector3 directionToPlayer = playerTransform.position - transform.position;
        float distanceToPlayer = directionToPlayer.magnitude;
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);

        if(distanceToPlayer <= visionRange && angleToPlayer < visionAngle * 0.5f)
        {
            //return true;

            if(playerTransform.position == lastTargetPosition)
            {
                lastTargetPosition = playerTransform.position;
                return true;
            }

            RaycastHit hit;
            if(Physics.Raycast(transform.position, directionToPlayer, out hit, distanceToPlayer))
            {
                if(hit.collider.CompareTag("Player"))
                {
                    return true;
                } 
            }

            return false;
        }

        return false;
    }

    void OnDrawGizmos() 
    {
        Gizmos.color = Color.blue;

        Gizmos.DrawWireCube(patrolAreaCenter.position, new Vector3(patrolAreaSize.x, 0, patrolAreaSize.y));

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, visionRange);

        Gizmos.color = Color.green;

        Vector3 fovLine1 = Quaternion.AngleAxis(visionAngle * 0.5f, transform.up) * transform.forward * visionRange;

        Vector3 fovLine2 = Quaternion.AngleAxis(-visionAngle * 0.5f, transform.up) * transform.forward * visionRange;

        Gizmos.DrawLine(transform.position, transform.position + fovLine1);

        Gizmos.DrawLine(transform.position, transform.position + fovLine2);
    }
}
