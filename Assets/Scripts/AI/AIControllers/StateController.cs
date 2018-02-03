using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class StateController : MonoBehaviour
{

    public State currentState;
    public EnemyStats enemyStats; //Need to set up enemy stats
    public Transform eyes; //Need eyes
    public State remainState; //Need remainInState


    [HideInInspector] public NavMeshAgent navMeshAgent;
    [HideInInspector] public EnemyAI enemyAI; 
    //[HideInInspector] public List<Transform> wayPointList;
    [HideInInspector] public int nextWayPoint;
    [HideInInspector] public Transform chaseTarget;
    [HideInInspector] public float stateTimeElapsed;

    private bool aiActive;


    void Awake()
    {
        enemyAI = GetComponent<EnemyAI>();
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        SetupAI(true);
    }

    public void SetupAI(bool aiActivationFromTankManager)
    //public void SetupAI(bool aiActivationFromTankManager, List<Transform> wayPointsFromTankManager)
    {
        //wayPointList = wayPointsFromTankManager;
        aiActive = aiActivationFromTankManager;
        if (aiActive)
        {
            navMeshAgent.enabled = true;
        }
        else
        {
            navMeshAgent.enabled = false;
        }
    }

    void Update()
    {
        if (!aiActive)
            return;
        currentState.UpdateState(this);
    }

    void OnDrawGizmos()
    {
        if (currentState != null && eyes != null)
        {
            Gizmos.color = currentState.sceneGizmoColor;
            Gizmos.DrawWireSphere(eyes.position, enemyStats.lookSphereCastRadius);
        }
    }

    public void TransitionToState(State nextState)
    {
        if (nextState != remainState)
        {
            currentState = nextState;
            OnExitState();
        }
    }

    public bool CheckIfCountDownElapsed(float duration)
    {
        stateTimeElapsed += Time.deltaTime;
        return (stateTimeElapsed >= duration);
    }

    private void OnExitState()
    {
        stateTimeElapsed = 0;
    }
}