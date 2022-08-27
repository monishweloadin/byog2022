using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 5;
    [SerializeField] private float _sprintSpeedMultiplier = 2;

    private CharacterController _characterController;
    private float _turnSpeed = 360;
    private Vector3 _input;

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
    }
}
