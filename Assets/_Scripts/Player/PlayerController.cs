using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private CharacterController _characterController;
    private float _moveSpeed = 5;
    private float _turnSpeed = 360;
    private Vector3 _input;

    private Animator _animator;


    private void Start()
    {
        _characterController = GetComponent<CharacterController>();  
        _animator = GetComponent<Animator>();
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
        if (_input == Vector3.zero) 
        {
            //_animator.SetTrigger("Idle");
            _animator.SetBool("IsWalking", false);
            _animator.SetBool("IsIdle", true);
            return; 
        }

        //_animator.SetTrigger("Walk");
        
        Quaternion rot = Quaternion.LookRotation(_input.ToIso(), Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, rot, _turnSpeed * Time.deltaTime);
    }

    private void Move()
    {
        _animator.SetBool("IsWalking", true);
        _animator.SetBool("IsIdle", false);
        _characterController.Move(transform.forward * _input.normalized.magnitude * _moveSpeed * Time.fixedDeltaTime);
    }
}
