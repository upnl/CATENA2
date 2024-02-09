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
    public float rollPower;

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
    public float speedByDash;
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
    public bool isFacingRight;
    
    // check player's state;
    [Header("- is****")] 
    public bool isJumping;
    public bool isRolling;
    public bool isAttacking;
    public bool isHit;
    public bool isAirHit;
    
    // help variables to check player's state; 
    [Header("- help to check player's state")] 
    public float rollingElapsedTime;
    public float rollingDoneCheckTime;
    
    public float jumpingElapsedTime;
    public float jumpingDoneCheckTime;

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
    public LayerMask groundLayer; // ground's layer num

    public float friction;
    public float rollFriction;

    #endregion

    #region Event functions
    
    private void Start()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _animator = GetComponentInChildren<Animator>();
        _collider = GetComponent<CapsuleCollider2D>();
    }

    /*
     * concerned with physics movement :
     * check if is grounded or in air, so check surroundings;
     * Update speedBy**** variables;
     * Update actual rigidbody velocity by above variables and surroundings, which means apply actual change on player;
     */
    private void FixedUpdate()
    {
        CheckSurroundings();
        UpdateSpeedByMove();
        UpdateSpeedByRoll();
        UpdateSpeedByDash();
        UpdateSpeedByHit();
        ApplyMovement();
    }

    /*
     * Update Animation States; which contains direction of playerGFX;
     * Update can**** variables;
     * -- Update player's state variables;
     * -- Update State;
     */
    private void Update()
    {
        isRolling = _animator.GetCurrentAnimatorStateInfo(0).IsTag("rolling");
        isAttacking = _animator.GetCurrentAnimatorStateInfo(0).IsTag("attack");
        
        UpdateAnimationParameters();
        if (canMove) UpdateGfxDirection();
        UpdateCanVariables();
        CheckTimeElapsed(ref jumpingElapsedTime, ref isJumping);
        
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
            _rigidbody2D.velocity = new Vector2(_rigidbody2D.velocity.x, 0);
            _rigidbody2D.AddForce(new Vector2(0, jumpPower), ForceMode2D.Impulse);
            isGrounded = false;
            isJumping = true;
            canJump = false;
            jumpingElapsedTime = jumpingDoneCheckTime;
            
            UpdateCanVariables();
        }
    }
    
    public void OnRoll(InputAction.CallbackContext value)
    {
        if (value.started && canRoll)
        {
            speedByRoll = (isFacingRight? 1 : -1) * rollPower;
            isRolling = true;
            rollingElapsedTime = rollingDoneCheckTime;
            _animator.SetTrigger("roll");
            
            UpdateCanVariables();
        }
    }
    
    
    #endregion

    #region physics update functions
    private void CheckSurroundings()
    {
        RaycastHit2D hit = Physics2D.Raycast(footPos.position, Vector2.down, downFromFoot, groundLayer);
        Debug.DrawRay(footPos.position,new Vector3(0,-1 * downFromFoot,0), new Color(0,1,0)); 
        Debug.DrawRay(hit.point, groundNormalPerp, new Color(1, 0, 0));

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
                if (_rigidbody2D.velocity.y <= 0)
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
        else
        {
            onSlope = false;
            isGrounded = false;
        }
    }

    private void UpdateSpeedByMove()
    {
        if (currentMoveDirection.x != 0 && canMove)
        {
            speedByMove += accel * currentMoveDirection.x;
        }
        else
        {
            speedByMove = Mathf.Lerp(speedByMove, 0, friction * Time.fixedDeltaTime);
        }
        
        ClampSpeedByMove();
    }

    private void ClampSpeedByMove()
    {
        if (Mathf.Abs(speedByMove) > speed)
        {
            speedByMove = (speedByMove / Mathf.Abs(speedByMove)) * speed;
        }
    }
    
    /**
     * currently, UpdateSpeedBy**** functions below are just applying lerp to each variables;
     * I'll fix this later for game purpose...
     */
    private void UpdateSpeedByRoll()
    {
        speedByRoll = Mathf.Lerp(speedByRoll, 0, rollFriction * Time.fixedDeltaTime);

        if (isRolling)
        {
            gameObject.layer = 8; // player rolling
        }
        else
        {
            gameObject.layer = 3; // player
        }
    }
    private void UpdateSpeedByDash()
    {
        speedByDash = Mathf.Lerp(speedByDash, 0, friction * Time.fixedDeltaTime);
    }
    
    private void UpdateSpeedByHit()
    {
        speedByHit = Mathf.Lerp(speedByHit, 0, friction * Time.fixedDeltaTime);
    }

    private void ApplyMovement()
    {
        currentVelocity.Set(speedByMove + speedByRoll + speedByDash + speedByHit, _rigidbody2D.velocity.y);
        _rigidbody2D.velocity = currentVelocity;
    }
    
    #endregion

    #region update functions
    private void UpdateAnimationParameters()
    {
        _animator.SetFloat("speedByMoving", speedByMove);
        _animator.SetFloat("yspeed", _rigidbody2D.velocity.y);
        _animator.SetBool("isGrounded", isGrounded);
    }

    public void UpdateGfxDirection()
    {
        if (currentMoveDirection.x > 0 && !isFacingRight)
        {
            isFacingRight = true;
            transform.Rotate(0.0f, 180.0f, 0.0f);
        }
        else if (currentMoveDirection.x < 0 && isFacingRight)
        {
            isFacingRight = false;
            transform.Rotate(0.0f, 180.0f, 0.0f);
        }
    }

    public void UpdateCanVariables()
    {
        UpdateCanMove();
        UpdateCanJump();
        UpdateCanRoll();
    }

    private void UpdateCanMove()
    {
        if (canMove)
        {
            if (isRolling || isAttacking) canMove = false;
        } else if (!isRolling && !isAttacking) canMove = true;
    }
    private void UpdateCanJump()
    {
        if (canJump)
        {
            if (!isGrounded || isAttacking) canJump = false;
            if (isRolling) canJump = false;
        } else if (!isJumping && isGrounded && !isRolling && !isAttacking) canJump = true;
    }
    private void UpdateCanRoll()
    {
        if (canRoll)
        {
            if (!isGrounded || isAttacking) canRoll = false;
            if (isRolling) canRoll = false;
        } else if (isGrounded && !isRolling && !isAttacking) canRoll = true;
    }
    private void CheckTimeElapsed(ref float timeElapsed, ref bool state)
    {
        if (timeElapsed > 0)
        {
            timeElapsed -= Time.deltaTime;
            if (timeElapsed <= 0)
            {
                timeElapsed = 0;
                state = false;
            }
        }
    }
    
    #endregion
    
    #region public methods
    
    public void Dash(float dashPower)
    {
        speedByDash = (isFacingRight? 1 : -1) * dashPower;
    }

    /*
     * method : Hit(float damage, Vector2 knockback);
     * parameters
     * - float damage : how much damage on player; we have to calculate real damage on player in this method using def, buff, etc later;
     * - Vector2 knockback : basically, AddForce(knockback.x * (player.x < monster.x ? -1 : 1),  tmp);
     * - float stunTime : stunTime, literally;
     */
    public void Hit(float damage, Vector2 knockback, float stunTime)
    {
        
    }
    
    #endregion
}
