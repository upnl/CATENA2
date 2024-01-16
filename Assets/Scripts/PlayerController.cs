using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class PlayerController : MonoBehaviour
{
    public enum PlayerState
    {
        Idle, Walk, Run, MovingUp, Falling, Attack, Jump, Roll, Hit, AirHit
    }
    
    #region declare member variables
    
    // player's properties, characteristics;
    [Header("- Player's Properties")]
    public float speed; // It means max-speed player can have;
    public float accel;
    public float jumpPower;

    public float maximumSlopeAngle;

    // player's components;
    private Rigidbody2D _rigidbody2D;
    private Animator _animator;
    private CapsuleCollider2D _collider;

    
    // store player's current state, value, or some;
    [Header("- Player's current states")]
    public PlayerState playerState;
    [Header("- speedBy****")]
    public float speedByMove;
    public float speedByRoll;
    public float speedByHit;
    public float speedByAttack;
    [Space]
    public Vector2 currentVelocity;
    public Vector2 currentMoveDirection;
    [Space]
    [Header("- checking surroundings")]
    public bool isGrounded;
    public bool onSlope;
    public Vector2 groundNormalPerp;
    public Vector2 groundNormal;
    public bool hitAir;
    public float afterJump;
    
    // check if player is able to do;
    [Header("- can****")]
    public bool canMove;
    public bool canJump;
    public bool canRoll;
    
    // player's gameobject children (to specify the position of head, foot, and kind of wall check) 
    [Header("- gameobject children")]
    public Transform footPos;
    
    // variables to check ground, wall
    [Header("- to check surroundings")]
    public float downFromFoot;
    
    // tmp variables
    [Header("- tmp")]
    public int groundLayer; // ground's layer num

    #endregion

    #region Event functions
    
    private void Start()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _animator = GetComponentInChildren<Animator>();
        _collider = GetComponent<CapsuleCollider2D>();
    }

    /*
     * concerned with physical movement :
     * check if is grounded or in air, so check surroundings;
     * Update speedBy**** variables;
     * Update actual rigidbody velocity by above variables and surroundings, which means apply actual change on player;
     */
    private void FixedUpdate()
    {
        ApplyMovement();
    }

    /*
     * Update Animation States;
     * Update can**** variables;
     * Update State
     */
    private void Update()
    {
        throw new NotImplementedException();
    }
    
    #endregion

    #region inputsystem events
    
    public void OnMove(InputAction.CallbackContext value)
    {
        currentMoveDirection = value.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext value)
    {
        if (value.started && canJump)
        {
            _rigidbody2D.AddForce(new Vector2(0, jumpPower), ForceMode2D.Impulse);
        }
    }
    
    #endregion

    private void CheckSurroundings()
    {
        RaycastHit2D hit = Physics2D.Raycast(footPos.position, Vector2.down, downFromFoot, groundLayer);
        Debug.DrawRay(hit.point, groundNormalPerp);

        if (hit)
        {
            groundNormalPerp = Vector2.Perpendicular(hit.normal);
            if (isGrounded)
            {
                // no slope, on flat ground
                if (groundNormalPerp.y == 0)
                {
                    onSlope = false;
                    isGrounded = true;
                } 
                // available slope
                else if (Vector2.Angle(Vector2.up, hit.normal) < maximumSlopeAngle)
                {
                    onSlope = true;
                    isGrounded = true;
                }
                // over maximum slope angle
                else
                {
                    onSlope = false;
                    isGrounded = false;
                }
            }
            else
            {
                if (_rigidbody2D.velocity.y <= 0 || hit.collider.GetComponent<PlatformEffector2D>() == null)
                {
                    // no slope, on flat ground
                    if (groundNormalPerp.y == 0)
                    {
                        onSlope = false;
                        isGrounded = true;
                    } 
                    // available slope
                    else if (Vector2.Angle(Vector2.up, hit.normal) < maximumSlopeAngle)
                    {
                        onSlope = true;
                        isGrounded = true;
                    }
                }
            }
        }
    }

    private void ApplyMovement()
    {
        currentVelocity.Set(currentMoveDirection.x * speed, _rigidbody2D.velocity.y);
        _rigidbody2D.velocity = currentVelocity;
    }
}
