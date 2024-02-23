using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI; //Para usar la funcion de unity de la IA

//Descargar IA en el package manager con nombre de AI Navigation buscar en window y sacar la pestaña de AI.

//Hay que hacer un bake del nav Mesh agent y crear en la escena un navmeshsurface

//Hay que descargar el probuilder desde el package manager o ya esta instalado

//El Probuilder sale en la pestaña de tools y le damos a la probuilder window, ahi te sale una ventana con cosas que puedas hacer al darle a new shape
//Shift en una cara para extruir 

public class RepasoExamen : MonoBehaviour
{
    //Maquina de estados
    enum State
    {
        Patrolling,
        Chasing,
        Attacking
    }
    
    [SerializeField]private State currentState; //estado actual de la IA
    

    NavMeshAgent agent; //hay que agregarle en el inspector el navmeshagent al enemigo

    Transform player;

    [SerializeField] Transform[] patrolPoints; //array para puntos(crear emptys con puntos a los que quiero que vaya la IA)

    [SerializeField] float detectionRange = 15;

    [SerializeField] float attackRange = 5;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Start is called before the first frame update
    void Start()
    {
        SetRandomPoint();
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
            case State.Attacking:
                Attack();
            break;
        }
    }

    void Patrol()
    {
        if(IsInRange(detectionRange) == true)
        {
            currentState = State.Chasing;
        }
        if(agent.remainingDistance < 0.5f)
        {
            SetRandomPoint(); //para que la IA patrulle en unos puntos.
        }
    }

    void SetRandomPoint()
    {
        agent.destination = patrolPoints[Random.Range(0, patrolPoints.Length -1)].position;
    }

    void Chase()
    {
        if(IsInRange(detectionRange) == false)
        {
            SetRandomPoint();
            currentState = State.Patrolling;
        }

        if(IsInRange(attackRange) == true)
        {
            currentState = State.Attacking;
        }

        agent.destination = player.position;
    }

    bool IsInRange(float range)
    {
        if(Vector3.Distance(transform.position, player.position) < range)
        {
            return true;
        }

        else
        {
            return false;
        }
    }

    void Attack()
    {
        Debug.Log("Atacando");

        currentState = State.Chasing;
    }

    void OnDrawGizmos() // variable por si quiero ver lo que esta haciendo el personaje dibujado
    {
        Gizmos.color = Color.blue;

        foreach (Transform point in patrolPoints)
        {
            Gizmos.DrawWireSphere(point.position, 0.5f);
        }

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

    }
}
