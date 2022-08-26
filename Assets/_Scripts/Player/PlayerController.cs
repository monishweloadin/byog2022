using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody _rb;
    private float _moveSpeed = 5;
    private float _turnSpeed = 360;
    private Vector3 _input;


    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
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
        _rb.MovePosition(transform.position + (transform.forward * _input.normalized.magnitude) * _moveSpeed * Time.deltaTime);
    }

}
