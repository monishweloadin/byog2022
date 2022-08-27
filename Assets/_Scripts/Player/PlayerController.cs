using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 5;
    [SerializeField] private float _sprintSpeedMultiplier = 2;
    [SerializeField] private float _jumpHeight = 3;

    [SerializeField] private bool _isGrounded = true;
    [SerializeField] private bool _isBlocking = true;

    private int _punchCombo = 0;

    private CharacterController _characterController;
    private float _turnSpeed = 360;
    private Vector3 _input;

    private float _gravity = -9.81f;

    private Animator _animator;




    private void Start()
    {
        _isBlocking = false;
        _characterController = GetComponent<CharacterController>();  
        _animator = GetComponent<Animator>();

    }

    private void Update()
    {
        CheckForGrounded();
        GetInput();
        Look();

        if (Input.GetMouseButtonDown(0))
        {
            Punch();
        }

        Move();
        CheckForJump();

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

        if (_animator.GetCurrentAnimatorStateInfo(0).IsName("PlayerLeftJab") || _animator.GetCurrentAnimatorStateInfo(0).IsName("PlayerRightJab"))
            inputMagnitude = 0;

        if (Input.GetKey(KeyCode.LeftShift))
        {
            inputMagnitude *= _sprintSpeedMultiplier;
        }

        _animator.SetFloat("MovementBlend", inputMagnitude);

        _characterController.Move(transform.forward * inputMagnitude * _moveSpeed * Time.deltaTime);
        
    }


    public Vector3 playerVelocity;
    private void CheckForJump()
    {
        if (_isGrounded && playerVelocity.y < -1f)
            playerVelocity.y = 0;

        if (Input.GetKeyDown(KeyCode.Space) && _isGrounded)
        {
            playerVelocity.y += Mathf.Sqrt(_jumpHeight * -3f * _gravity);
        }

        playerVelocity.y += _gravity * 10 * Time.deltaTime;
        _characterController.Move(playerVelocity * Time.deltaTime);
    }

    private void CheckForGrounded()
    {
        Debug.DrawRay(transform.position, Vector3.down, Color.green);
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 0.3f))
        {             
            if (hit.transform.CompareTag("Ground"))
            {
                _isGrounded = true;
            }
        }
        else
            _isGrounded = false;
    }

    private void Punch()
    {
        
        switch(_punchCombo)
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
        _characterController.Move(Vector3.zero);

    }
}
