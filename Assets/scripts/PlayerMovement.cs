using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D _rb;
    private Animator _animator;
    private Vector2 _goal;
    public float goal_threshold;
    public float speed;
    private float _angle = 0;
    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _goal = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            _animator.SetBool("isMoving", true);
        }
    }


    void FixedUpdate()
    {
        Vector2 direction = _goal - _rb.position;
        _rb.velocity = speed * direction.normalized;
        
        if (direction.magnitude < goal_threshold)
        {
            _rb.velocity = Vector2.zero;
            _animator.SetBool("isMoving", false);
        }
        
        //(float) (Mathf.Atan2(direction.y - transform.up.y, direction.x) * 180 / Math.PI); //
        _angle = Vector2.Angle(direction, - transform.up);
        
        if (_rb.velocity.x < 0)
        {
            _angle = 360 - _angle;
        }
        _animator.SetFloat("angle", _angle);
    }
}
