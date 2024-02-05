using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class HeroKnightCombatController : MonoBehaviour
{
    // Player's Components;
    private PlayerController _playerController;
    private Animator _animator;
    private Rigidbody2D _rigidbody;
    
    
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

    public void OnAttack(InputAction.CallbackContext value)
    {
        if (canAttack && value.started)
        {
            _playerController.UpdateGfxDirection();
            _animator.SetTrigger("attack");
            _playerController.Dash(5f);
            canAttack = false;
            attackCheckElapsedTime = 0.1f;
            Debug.Log("CANATTACK IS FALSE NOW!");
            
            _playerController.UpdateCanVariables();
        }
    }

    private void UpdateCanAttack()
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
}
