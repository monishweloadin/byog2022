using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 5;
    [SerializeField] private float _sprintSpeedMultiplier = 2;

    [SerializeField] private float _minChaseDistance = 10;
    [SerializeField] private float _minAttackDistance = 10;

    public GameObject Player;
    
    private bool _isGrounded = true;

    private NavMeshAgent _navMeshAgent;
    private NavMeshPath _path;
    

    private Animator _animator;

    private EnemyState _enemyState;

    private void Start()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _path = new NavMeshPath();
        _animator = GetComponent<Animator>();

        _enemyState = EnemyState.IDLE;

        
    }

    private void Update()
    {        
        _navMeshAgent.CalculatePath(Player.transform.position, _path);
        if(_path.status != NavMeshPathStatus.PathComplete)
        {
            print("return");
            return;
        }

        if (PlayerWithinDistance(_path, _minAttackDistance))
        {
            ChangeState(EnemyState.ATTACK);
        }
        else if (PlayerWithinDistance(_path, _minChaseDistance))
        {
            ChangeState(EnemyState.CHASE);
        }
        else
        {
            ChangeState(EnemyState.IDLE);
        }

        if(_enemyState == EnemyState.CHASE)
            _navMeshAgent.destination = Player.transform.position;

    }


    public void ChangeState(EnemyState state)
    {
        if(state == _enemyState)
            return;

        switch (state)
        {
            case EnemyState.IDLE:

                break;
            case EnemyState.CHASE:
                break;
            case EnemyState.ATTACK:
                break;
        }
        _enemyState = state;
    }

    public float GetPathLength(NavMeshPath path)
    {
        float lng = 0.0f;

        if ((path.status != NavMeshPathStatus.PathInvalid) && (path.corners.Length > 1))
        {
            for (int i = 1; i < path.corners.Length; ++i)
            {
                lng += Vector3.Distance(path.corners[i - 1], path.corners[i]);
            }
        }

        return lng;
    }

    private bool PlayerWithinDistance(NavMeshPath path, float distanceThreshold)
    {
        if (GetPathLength(path) <= distanceThreshold)
            return true;
        else
            return false;
    }


    private void OnTriggerEnter(Collider other)
    {
        // reduce health
    }
}

public enum EnemyState
{
    IDLE,
    CHASE,
    ATTACK
}

