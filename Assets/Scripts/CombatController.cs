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
    
    // can****; indicate if player can do some action;
    public bool canAttack;
    
    // help variables to check player's state; 
    public float attackCheckElapsedTime;
    
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

    public void CanAttack()
    {
        canAttack = true;
    }
}
