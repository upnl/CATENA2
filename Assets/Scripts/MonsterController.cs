using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Serialization;

public class MonsterController : CreatureController
{
    #region declare member variables

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
    
    // monster's components;
    private MonsterCombatController _monsterCombatController;
    
    // store monster's current state

    [Header("- to chase player")] 
    public bool isChasingPlayer;
    public float playerDetectTime;
    public float playerDetectTimeElapsed;
    public GameObject playerGameObject;

    // tmp variables
    [Header("- tmp")]
    public LayerMask playerLayer;
    public Collider2D[] playerDetectColsArray;
    
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
    protected override void Start()
    {
        base.Start();
        _monsterCombatController = GetComponent<MonsterCombatController>();
    }
    
    /*
     * concerned with physics movement :
     * check if is grounded or in air, so check surroundings;
     * Update speedBy**** variables;
     * Update actual rigidbody velocity by above variables and surroundings, which means apply actual change on player;
     */

    /*
     * Update Animation States; which contains direction of playerGFX;
     * Update can**** variables;
     * Update monster's Next Action
     * -- Update State;
     */
    protected override void Update()
    {
        isAttacking = Animator.GetCurrentAnimatorStateInfo(0).IsTag("attack");

        UpdateAnimationParameters();
        if (canMove) UpdateGfxDirection();
        UpdateCanVariables();
        UpdateNextAction();
        UpdateHitState();
        CheckTimeElapsed(ref jumpingElapsedTime, ref isJumping);
        CheckTimeElapsed(ref stunTimeElapsed, ref isStun);
    }
    #endregion

    #region update functions
    
    protected override bool ConditionForUpdateCanVariable()
    {
        return isAttacking || isHit;
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
                    _monsterCombatController.OnAttack(0);
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

    #endregion
}
