using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    public float Health = 100;

    public float HealthReducePerHit = 10;

    public float HealthDecreaseMultiplier = 1;

    public bool CanReduceLifeOverTime = false;

    [SerializeField] private float _minChaseDistance = 10;
    [SerializeField] private float _minAttackDistance = 10;

    public GameObject RightHand;

    private GameObject _player;
    public bool ShowDistance;
    
    public NavMeshAgent EnemyNavMeshAgent;
    private NavMeshPath _path;
    

    private Animator _animator;

    private EnemyState _enemyState;

    public GameObject CurrentlyHoldingItem;

    [Header("UI")]
    public Slider HealthUI;

    public bool CanMove;

    private bool _isDead;

    private void Start()
    {
        CanMove = true;
        _player = LevelManager.Instance.Player;
        EnemyNavMeshAgent = GetComponent<NavMeshAgent>();
        _path = new NavMeshPath();
        _animator = GetComponent<Animator>();

        EnemyNavMeshAgent.isStopped = false;

        ChangeState(EnemyState.IDLE);
    }

    public IEnumerator KillEnemy()
    {
        _isDead = true;
        _animator.SetTrigger("Died");
        EnemyNavMeshAgent.isStopped = true;
        yield return new WaitForSeconds(5f);
        gameObject.SetActive(false);
    }

    public void ResetEnemyMoving()
    {
        EnemyNavMeshAgent.isStopped = false;
    }

    public void ResetState()
    {
        ChangeState(_enemyState);
    }

    public void ChangeToIdle()
    {
        _animator.SetTrigger("Idle");
    }

    private void Update()
    {
        if (_isDead) return;
        if (!CanMove)
        {
            EnemyNavMeshAgent.isStopped = true;
            return;
        }

        UpdateSelfUI();


        EnemyNavMeshAgent.CalculatePath(_player.transform.position, _path);
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
            EnemyNavMeshAgent.destination = _player.transform.position;

        if (CanReduceLifeOverTime)
            DecreaseHealthOvertime();


        if (Health <= 0)
            StartCoroutine(KillEnemy());
    }

    public void UpdateSelfUI()
    {
        HealthUI.value = Health;
    }


    public void ChangeState(EnemyState state)
    {
        if(state == _enemyState)
            return;


        switch (state)
        {
            case EnemyState.IDLE:
                EnemyNavMeshAgent.isStopped = true;
                _animator.SetTrigger("Idle");
                break;
            case EnemyState.CHASE:
                EnemyNavMeshAgent.isStopped = false;
                _animator.SetTrigger("Walk");
                break;
            case EnemyState.ATTACK:
                EnemyNavMeshAgent.isStopped = false;
                if (CurrentlyHoldingItem != null)
                {
                    _animator.SetTrigger("WeaponHit");
                }
                else
                {
                    _animator.SetTrigger("Punch");
                }
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
        if (other.CompareTag("PlayerHit") && !_isDead)
        {
            _animator.SetTrigger("DamageTaken");
            if (other.transform.root.GetComponent<PlayerController>().CurrentObjectOnHand != null)
            {
                if (other.transform.root.GetComponent<PlayerController>().CurrentObjectOnHand.GetComponent<PickableObject>().PickableType == PickableType.KEY)
                {
                    ReduceHealth(HealthReducePerHit);
                }
                else
                {
                    other.transform.root.GetComponent<PlayerController>().ReduceHealth(10);

                    GameObject obj = other.transform.root.GetComponent<PlayerController>().CurrentObjectOnHand;
                    other.transform.root.GetComponent<PlayerController>().CurrentObjectOnHand = null;

                    if (CurrentlyHoldingItem != null)
                    {
                        CurrentlyHoldingItem.transform.parent = null;

                        CurrentlyHoldingItem.GetComponent<CapsuleCollider>().enabled = true;
                        CurrentlyHoldingItem.GetComponent<BoxCollider>().enabled = true;
                        CurrentlyHoldingItem.GetComponent<Rigidbody>().isKinematic = false;
                        CurrentlyHoldingItem.GetComponent<PickableObject>().CanPickup = true;
                    }

                    PickupObject(obj);
                }
            }
        }

        if (other.CompareTag("PickableObject"))
        {
            if (CurrentlyHoldingItem == null)
            {
                GameObject obj = other.gameObject;
                PickupObject(obj);
            }
        }
    }

    private void PickupObject(GameObject obj)
    {
        if(LevelManager.Instance.AvalaiblePickupObjects.Contains(obj))
            LevelManager.Instance.AvalaiblePickupObjects.Remove(obj);

        obj.GetComponent<CapsuleCollider>().enabled = false;
        obj.GetComponent<BoxCollider>().enabled = false;
        obj.GetComponent<Rigidbody>().isKinematic = true;
        obj.GetComponent<PickableObject>().CanPickup = false;

        obj.transform.SetParent(this.RightHand.transform);
        obj.transform.localPosition = obj.GetComponent<PickableObject>().PostionOffset;
        obj.transform.localRotation = Quaternion.Euler(obj.GetComponent<PickableObject>().ObjectRotation);

        CurrentlyHoldingItem = obj;

        UIController.Instance.EnablePickupUI(false);
    }

    public void AddHealth(float value)
    {
        Health = Mathf.Clamp(Health + value, 0, 100);
    }

    public void ReduceHealth(float value)
    {
        Health = Mathf.Clamp(Health - value, 0, 100);
    }

    private float healthTimeElapsed = 0;
    private void DecreaseHealthOvertime()
    {
        healthTimeElapsed += Time.deltaTime;

        if (healthTimeElapsed >= HealthDecreaseMultiplier)
        {
            healthTimeElapsed %= HealthDecreaseMultiplier;
            ReduceHealth(1);
        }
    }

}

public enum EnemyState
{
    IDLE,
    CHASE,
    ATTACK
}

