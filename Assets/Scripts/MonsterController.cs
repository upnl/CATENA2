using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Serialization;

public class MonsterController : MonoBehaviour
{
    #region declare member variables
    
    // monster's properties;
    [Header("- Monster's Properties")] 
    public float speed;
    public float accel;
    public float jumpPower;
    
    /*
     * we need...
     * - player detect box;
     * - attackTriggerBox
     * - change Action interval
     * - --attack knockback
     * - --atk
     * - --etc.
     */

    public Vector2 playerDetectBox; // when player comes into this box, monster's gonna chase;
    public Vector2 attackTriggerBox; // when player comes into this box, monster's gonna attack;
    public float changeActionInterval;
    public float changeActionIntervalElapsed;
    
    
    public float maximumSlopeAngle;
    
    // monster's components;
    private Rigidbody2D _rigidbody2D;
    private Animator _animator;
    private CapsuleCollider2D _collider;
    
    
    // store monster's current state
    [Header("- Monster's current states")]
    [Header("- speedBy****")]
    public float speedByMove;
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
    [Space] 
    
    [Header("- to chase player")] 
    public bool isChasingPlayer;
    public float playerDetectTime;
    public float playerDetectTimeElapsed;
    public GameObject playerGameObject;

    [Space] [Header("- combat interaction")]
    public float hitTimeElapsed;
    public float stunTimeElapsed;
    
    // check monster's state;
    [Header("- is****")] 
    public bool isJumping;
    public bool isAttacking;
    public bool isStun;
    public bool isHit;
    public bool isAirHit;
    
    // help variables to check monster's state; 
    [Header("- help to check player's state")]
    public float jumpingElapsedTime;
    public float jumpingDoneCheckTime;

    // check if monster is able to do;
    [Header("- can****")]
    public bool canMove;
    public bool canJump;

    // monster's gameobject children (to specify the position of head, foot, and kind of wall check) 
    [Header("- gameobject children")]
    public Transform footPos;
    
    // variables to check ground, wall
    [Header("- to check surroundings")]
    public float downFromFoot;
    
    // tmp variables
    [Header("- tmp")]
    public LayerMask groundLayer; // ground's layer num
    public LayerMask playerLayer;
    public Collider2D[] playerDetectColsArray;

    public float friction;

    #endregion
    
    #region Event functions
    
    /*
     * for checking box sizes, etc in editing level;
     */
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube((Vector2)transform.position, playerDetectBox);
        Gizmos.DrawWireCube((Vector2)transform.position, attackTriggerBox);
    }
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
        UpdateSpeedByDash();
        UpdateSpeedByHit();
        ApplyMovement();
    }
    
    /*
     * Update Animation States; which contains direction of playerGFX;
     * Update can**** variables;
     * Update monster's Next Action
     * -- Update State;
     */
    private void Update()
    {
        isAttacking = _animator.GetCurrentAnimatorStateInfo(0).IsTag("attack");

        UpdateAnimationParameters();
        if (canMove) UpdateGfxDirection();
        UpdateCanVariables();
        UpdateNextAction();
        UpdateHitState();
        CheckTimeElapsed(ref jumpingElapsedTime, ref isJumping);
        CheckTimeElapsed(ref stunTimeElapsed, ref isStun);
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
                        Time.timeScale = 1f;
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
    private void UpdateSpeedByDash()
    {
        speedByDash = Mathf.Lerp(speedByDash, 0, friction * Time.fixedDeltaTime);
    }
    
    private void UpdateSpeedByHit()
    {
        if (isGrounded) speedByHit = Mathf.Lerp(speedByHit, 0, friction * Time.fixedDeltaTime);
    }

    private void ApplyMovement()
    {
        currentVelocity.Set(speedByMove + speedByDash + speedByHit, _rigidbody2D.velocity.y);
        _rigidbody2D.velocity = currentVelocity;
    }
    
    #endregion
    
    #region update functions
    private void UpdateAnimationParameters()
    {
        _animator.SetFloat("speedByMoving", speedByMove);
        _animator.SetFloat("yspeed", _rigidbody2D.velocity.y);
        _animator.SetBool("isGrounded", isGrounded);
        _animator.SetBool("isHit", isHit);
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
    }

    private void UpdateCanMove()
    {
        if (canMove)
        {
            if (isAttacking || isHit) canMove = false;
        } else if (!isAttacking && !isHit) canMove = true;
    }
    private void UpdateCanJump()
    {
        if (canJump)
        {
            if (!isGrounded || isAttacking || isHit) canJump = false;
        } else if (!isJumping && isGrounded  && !isAttacking && !isHit) canJump = true;
    }

    private void UpdateNextAction()
    {
        changeActionIntervalElapsed -= Time.deltaTime;
        playerDetectTimeElapsed -= Time.deltaTime;
        
        if (changeActionIntervalElapsed <= 0)
        {
            changeActionIntervalElapsed = changeActionInterval;
            playerDetectColsArray = Physics2D.OverlapBoxAll(transform.position, 
                playerDetectBox, 0, playerLayer);

            if (isChasingPlayer)
            {
                // if detect the player, keep chasing;
                if (playerDetectColsArray.Length != 0) playerDetectTimeElapsed = playerDetectTime;
                else if (playerDetectTimeElapsed <= 0)
                {
                    isChasingPlayer = false;
                    playerGameObject = null;
                }
                
                // if the player is in box of triggerAttackBox, attack;
                // otherwise, just chase;
                
                playerDetectColsArray = Physics2D.OverlapBoxAll(transform.position, 
                    attackTriggerBox, 0, playerLayer);
                
                if (playerDetectColsArray.Length != 0)
                {
                    // attack;
                }
                else
                {
                    if (playerGameObject != null && playerGameObject.transform.position.x < transform.position.x)
                    {
                        currentMoveDirection = Vector2.left;
                    }
                    else
                    {
                        currentMoveDirection = Vector2.right;
                    }
                }
            }
            else
            {
                if (playerDetectColsArray.Length != 0)
                {
                    playerDetectTimeElapsed = playerDetectTime;
                    isChasingPlayer = true;
                    playerGameObject = playerDetectColsArray[0].gameObject;
                }
                else
                {

                    int random = Random.Range(0, 3);

                    if (random == 0)
                    {
                        currentMoveDirection = Vector2.left;
                    }
                    else if (random == 1)
                    {
                        currentMoveDirection = Vector2.zero;
                    }
                    else
                    {
                        currentMoveDirection = Vector2.right;
                    }
                }
            }
        }
    }

    private void UpdateHitState()
    {
        if (isGrounded)
        {
            hitTimeElapsed -= Time.deltaTime;
            if (hitTimeElapsed <= 0 && !isStun)
            {
                isHit = false;
            }
        }
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
    
    /*
     * method : Hit(float damage, Vector2 knockback);
     * parameters
     * - float damage : how much damage on player; we have to calculate real damage on player in this method using def, buff, etc later;
     * - Vector2 knockback : basically, AddForce(knockback.x * (player.x < monster.x ? -1 : 1),  tmp);
     * - float stunTime : stunTime, literally;
     * - int direction : -1 - player.x < monster.x, 1 - otherwise
     */
    public void Hit(float damage, Vector2 knockback, float stunTime, int direction)
    {
        isHit = true;
        if (stunTime > 0)
        {
            stunTimeElapsed = stunTime;
            isStun = true;
        }

        currentMoveDirection.x = direction;
        UpdateGfxDirection();
        speedByHit = knockback.x * (transform.rotation.y < 0 ? -1 : 1);

        _rigidbody2D.velocity = new Vector2(_rigidbody2D.velocity.x, 0);
        _rigidbody2D.AddForce(new Vector2(0, knockback.y), ForceMode2D.Impulse);
        isGrounded = false;
        isJumping = true;
        canJump = false;
        jumpingElapsedTime = jumpingDoneCheckTime;
        
        UpdateCanVariables();
    }
    
    #endregion
}
