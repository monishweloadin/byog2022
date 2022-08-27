using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    public float Health = 100;

    [SerializeField] private float _minChaseDistance = 10;
    [SerializeField] private float _minAttackDistance = 10;

    private GameObject _player;
    public bool ShowDistance;
    
    private NavMeshAgent _navMeshAgent;
    private NavMeshPath _path;
    

    private Animator _animator;

    private EnemyState _enemyState;

    private Coroutine _punchingCoroutine;

    private void Start()
    {
        _player = LevelManager.Instance.Player;
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _path = new NavMeshPath();
        _animator = GetComponent<Animator>();

        ChangeState(EnemyState.IDLE);

    }

    private void Update()
    {        
        _navMeshAgent.CalculatePath(_player.transform.position, _path);
        if(_path.status != NavMeshPathStatus.PathComplete)
        {
            print("return");
            return;
        }

        if (ShowDistance)
            GetPathLength(_path);

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
            _navMeshAgent.destination = _player.transform.position;

    }


    public void ChangeState(EnemyState state)
    {
        if(state == _enemyState)
            return;

        if (state != EnemyState.ATTACK && _punchingCoroutine != null)
            StopCoroutine(_punchingCoroutine);


        switch (state)
        {
            case EnemyState.IDLE:
                _navMeshAgent.isStopped = true;
                _animator.SetTrigger("Idle");
                break;
            case EnemyState.CHASE:
                _navMeshAgent.isStopped = false;
                _animator.SetTrigger("Walk");
                break;
            case EnemyState.ATTACK:
                _navMeshAgent.isStopped = false;
                _animator.SetTrigger("Punch");
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
        if (other.CompareTag("PlayerHit"))
        {
            _animator.SetTrigger("DamageTaken");
        }
    }


    public void PunchedPlayer()
    {
        if (_player.transform.GetComponent<PlayerController>().IsBlocking)
        {
            _player.transform.GetComponent<Animator>().SetTrigger("DamageTakenBlock");
        }
        else
        {
            print("ouch");
            _player.transform.GetComponent<PlayerController>().AddHealth(10);
            _player.transform.GetComponent<Animator>().SetTrigger("DamageTaken");
        }
    }
}

public enum EnemyState
{
    IDLE,
    CHASE,
    ATTACK
}

