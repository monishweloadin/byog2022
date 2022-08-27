using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 5;
    [SerializeField] private float _sprintSpeedMultiplier = 2;
    [SerializeField] private float _jumpHeight = 3;

    private CharacterController _characterController;
    private float _turnSpeed = 360;
    private Vector3 _input;

    private float _gravity = -9.81f;

    private Animator _animator;


    private void Start()
    {
        _characterController = GetComponent<CharacterController>();  
        _animator = GetComponent<Animator>();

        transform.GetChild(0).transform.position = new Vector3(transform.GetChild(0).transform.position.x, -2.2f, transform.GetChild(0).transform.position.z);
    }

    private void Update()
    {
        GetInput();
        Look();
    }

    private void FixedUpdate()
    {
        Move();
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

        _characterController.Move(transform.forward * inputMagnitude * _moveSpeed * Time.fixedDeltaTime);

        CheckForJump();
    }

    private void CheckForJump()
    {
        Vector3 playerVelocity = Vector3.zero;

        print(_characterController.isGrounded);

        if(Input.GetKeyDown(KeyCode.Space) && _characterController.isGrounded)
        {
            print("jumpo");
            playerVelocity.y += Mathf.Sqrt(_jumpHeight * -1 * _gravity);
        }

        playerVelocity.y += _gravity * Time.fixedDeltaTime;

        _characterController.Move(playerVelocity * Time.fixedDeltaTime);
        
    }
}
