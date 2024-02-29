using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CombatController : MonoBehaviour
{
    // Player's Components;
    private PlayerController _playerController;
    private Animator _animator;
    private Rigidbody2D _rigidbody;
    private PlayerAnimationEvents _animationEvents;
    
    // Properties for Player's components;
    protected PlayerController PlayerController => _playerController;
    protected Animator Animator => _animator;
    protected Rigidbody2D Rigidbody => _rigidbody;
    protected PlayerAnimationEvents AnimationEvents => _animationEvents;
    
    // Player's attack information;

    public AttackInfo[] attackInfos;
    
    // can****; indicate if player can do some action;
    public bool canAttack; // ground attack;
    public bool canAirAttack; // air attack;
    
    // help variables to check player's state; 
    public float attackCheckElapsedTime;
    
    // store current states;
    public Vector2 offsetAccordingToPlayerDirection;
    public int currentAttackIndex; // the index of an attack cycle (like skill0, skill1, etc.)
    public int currentAttackNumber; // the number of attack in the same attack cycle (like first attack in skill1, second attack in skill1, etc.)

    // help player-monster combat interaction;
    public Collider2D[] attackCheckCols;
    public LayerMask monsterLayer;
    
    private void Start()
    {
        _playerController = GetComponent<PlayerController>();
        _animator = GetComponentInChildren<Animator>();
        _animationEvents = GetComponentInChildren<PlayerAnimationEvents>();
    }

    private void Update()
    {
        UpdateCanAttack();
        UpdateCanAirAttack();
        if (attackCheckElapsedTime > 0) attackCheckElapsedTime -= Time.deltaTime;
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        
        for (int i = 0; i<attackInfos.Length; i++)
        {
            for (int j = 0; j < attackInfos[i].AttackBoundaries.Length; j++)
            {
                Gizmos.color += Color.white * (0.3f / attackInfos.Length);

                offsetAccordingToPlayerDirection = attackInfos[i].AttackBoundaryOffsets[j];
                offsetAccordingToPlayerDirection.x *= (transform.rotation.y < 0 ? 1 : -1);
                Gizmos.DrawWireCube((Vector2)transform.position + offsetAccordingToPlayerDirection,
                    attackInfos[i].AttackBoundaries[j]);
            }
        }
    }
    
    protected virtual void UpdateCanAttack()
    {
        if (canAttack)
        {
            if (!_playerController.isGrounded || _playerController.isRolling || _playerController.isHit)
            {
                canAttack = false;
            }
        } else if (attackCheckElapsedTime <= 0 && _playerController.isGrounded && 
                   !_playerController.isRolling && !_playerController.isAttacking && !_playerController.isHit) canAttack = true;
    }
    
    protected virtual void UpdateCanAirAttack()
    {
        if (canAirAttack)
        {
            if (_playerController.isRolling || _playerController.isHit)
            {
                canAirAttack = false;
            }
        } else if (attackCheckElapsedTime <= 0 && !_playerController.isRolling && !_playerController.isAttacking && !_playerController.isHit) canAirAttack = true;
    }

    
    /* Hit Box Check;
     * function "Attack" is overloaded;
     * the one applys slow motion on the game and the other is not;
     */
    public void HitBoxCheck()
    {
        AttackInfo attackInfo = attackInfos[currentAttackIndex];
        
        offsetAccordingToPlayerDirection = attackInfo.AttackBoundaryOffsets[currentAttackNumber];
        offsetAccordingToPlayerDirection.x *= (transform.rotation.y < 0 ? 1 : -1);
        attackCheckCols = Physics2D.OverlapBoxAll((Vector2)
            transform.position + offsetAccordingToPlayerDirection, 
            attackInfo.AttackBoundaries[currentAttackNumber], 0, monsterLayer);

        foreach (var i in attackCheckCols)
        {
            if (i.CompareTag("Monster"))
            {
                CreatureController creatureController = i.GetComponent<CreatureController>();
                int attackDirection = i.transform.position.x > transform.position.x ? -1 : 1;
                creatureController.Hit(
                    attackInfo.AttackDamages[currentAttackNumber], 
                    attackInfo.AttackKnockBacks[currentAttackNumber], 
                    attackInfo.AttackStunTimes[currentAttackNumber], attackDirection,
                    attackInfo.SlowMotionInfos[currentAttackNumber].slowMotionRate,
                    attackInfo.SlowMotionInfos[currentAttackNumber].slowMotionTime);
            }
        }
    }
    public void CanAttack()
    {
        canAttack = true;
        canAirAttack = true;
    }

    public void UnableCanAttacks()
    {
        canAttack = false;
        canAirAttack = false;
    }

    public void Dash(int index)
    {
        _playerController.Dash(attackInfos[currentAttackIndex].AttackDashes[currentAttackNumber]);
    }
}
