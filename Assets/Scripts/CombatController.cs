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
    
    // Player's attack informations
    [Serializable]
    public class SlowMotionInfo
    {
        public float slowMotionRate;
        public float slowMotionTime;
    }
    public Vector2[] attackBoundaries;
    public Vector2[] attackBoundaryOffsets;
    public Vector2[] attackKnockBacks;
    public float[] attackDamages;
    public float[] attackStunTimes;
    public float[] attackDashes;
    public SlowMotionInfo[] slowMotionInfos;
    
    // can****; indicate if player can do some action;
    public bool canAttack; // ground attack;
    public bool canAirAttack; // air attack;
    
    // help variables to check player's state; 
    public float attackCheckElapsedTime;
    
    // store current states;
    public Vector2 offsetAccordingToPlayerDirection;
    public int currentAttackIndex;

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
        
        for (int i = 0; i<attackBoundaries.Length; i++)
        {
            offsetAccordingToPlayerDirection = attackBoundaryOffsets[i];
            offsetAccordingToPlayerDirection.x *= (transform.rotation.y < 0 ? 1 : -1);
            Gizmos.DrawWireCube((Vector2) transform.position + offsetAccordingToPlayerDirection, 
                attackBoundaries[i] );
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

    public void Attack(int index)
    {
        offsetAccordingToPlayerDirection = attackBoundaryOffsets[index];
        offsetAccordingToPlayerDirection.x *= (transform.rotation.y < 0 ? 1 : -1);
        attackCheckCols = Physics2D.OverlapBoxAll((Vector2)
            transform.position + offsetAccordingToPlayerDirection, 
            attackBoundaries[index], 0, monsterLayer);

        foreach (var i in attackCheckCols)
        {
            if (i.CompareTag("Monster"))
            {
                CreatureController creatureController = i.GetComponent<CreatureController>();
                int attackDirection = i.transform.position.x > transform.position.x ? -1 : 1;
                creatureController.Hit(attackDamages[index], attackKnockBacks[index], attackStunTimes[index], attackDirection);
            }
        }
    }
    
    public void Attack(int index, float slowMotionRate, float slowMotionTime)
    {
        index = currentAttackIndex;
        
        offsetAccordingToPlayerDirection = attackBoundaryOffsets[index];
        offsetAccordingToPlayerDirection.x *= (transform.rotation.y < 0 ? 1 : -1);
        attackCheckCols = Physics2D.OverlapBoxAll((Vector2)
            transform.position + offsetAccordingToPlayerDirection, 
            attackBoundaries[index], 0, monsterLayer);

        foreach (var i in attackCheckCols)
        {
            if (i.CompareTag("Monster"))
            {
                CreatureController creatureController = i.GetComponent<CreatureController>();
                int attackDirection = i.transform.position.x > transform.position.x ? -1 : 1;
                creatureController.Hit(attackDamages[index], attackKnockBacks[index], attackStunTimes[index], attackDirection, slowMotionRate, slowMotionTime);
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
        _playerController.Dash(attackDashes[index]);
    }
}
