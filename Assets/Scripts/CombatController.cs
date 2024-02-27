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
    
    // Properties for Player's components;
    protected PlayerController PlayerController => _playerController;
    protected Animator Animator => _animator;
    protected Rigidbody2D Rigidbody => _rigidbody;
    
    // Player's attack informations
    public Vector2[] attackBoundaries;
    public Vector2[] attackBoundaryOffsets;
    public Vector2[] attackKnockBacks;
    public float[] attackDamages;
    public float[] attackStunTimes;
    
    // can****; indicate if player can do some action;
    public bool canAttack;
    
    // help variables to check player's state; 
    public float attackCheckElapsedTime;
    
    // store current states;
    public Vector2 offsetAccordingToPlayerDirection;

    // help player-monster combat interaction;
    public Collider2D[] attackCheckCols;
    public LayerMask monsterLayer;
    
    private void Start()
    {
        _playerController = GetComponent<PlayerController>();
        _animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        UpdateCanAttack();
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
            if (!_playerController.isGrounded || _playerController.isRolling)
            {
                canAttack = false;
            }
        } else if (attackCheckElapsedTime <= 0 && _playerController.isGrounded && 
                   !_playerController.isRolling && !_playerController.isAttacking) canAttack = true;
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

    public void CanAttack()
    {
        canAttack = true;
    }
}
