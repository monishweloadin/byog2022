using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 5;
    [SerializeField] private float _sprintSpeedMultiplier = 2;

    [SerializeField] private float _minChaseDistance = 10;
    [SerializeField] private float _minAttackDistance = 10;
    

    private bool _isGrounded = true;

    private int _punchCombo = 0;

    private CharacterController _characterController;
    private float _turnSpeed = 360;
    private Vector3 _input;

    private float _gravity = -9.81f;

    private Animator _animator;

    private EnemyState _enemyState;

    private void Start()
    {

        _characterController = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (PlayerWithinDistance(_minChaseDistance))
        {
            ChangeState(EnemyState.CHASE);
        }
        else if (PlayerWithinDistance(_minAttackDistance))
        {
            ChangeState(EnemyState.ATTACK);
        }
        else
        {
            ChangeState(EnemyState.IDLE);
        }
        
        ApplyGravity();
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

    private bool PlayerWithinDistance(float distanceThreshold)
    {
        return true;
    }


    private void GetInput()
    {
        _input = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
    }

    private void Look()
    {
        if (_input == Vector3.zero) return;

        Quaternion rot = Quaternion.LookRotation(_input.ToIso(), Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, rot, _turnSpeed * Time.deltaTime);
    }

    private void Move()
    {
        float inputMagnitude = _input.normalized.magnitude;


        if (Input.GetKey(KeyCode.LeftShift))
        {
            inputMagnitude *= _sprintSpeedMultiplier;
        }

        _animator.SetFloat("MovementBlend", inputMagnitude);

        _characterController.Move(transform.forward * inputMagnitude * _moveSpeed * Time.deltaTime);

    }



    private void Punch()
    {
        if (Input.GetMouseButtonDown(0))
        {

            switch (_punchCombo)
            {
                case 0:
                    _punchCombo = 1;
                    _animator.SetTrigger("Punch1");
                    break;
                case 1:
                    _punchCombo = 0;
                    _animator.SetTrigger("Punch2");
                    break;
            }

        }
    }


    public Vector3 enemyVelocity;
    private void ApplyGravity()
    {

        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 0.3f))
        {
            if (hit.transform.CompareTag("Ground"))
            {
                _isGrounded = true;
            }
        }
        else
        {
            _isGrounded = false;
        }

        if (_isGrounded && enemyVelocity.y < -1f)
            enemyVelocity.y = 0;


        enemyVelocity.y += _gravity * 10 * Time.deltaTime;
        _characterController.Move(enemyVelocity * Time.deltaTime);
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

