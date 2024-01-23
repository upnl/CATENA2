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

    private void Start()
    {
        _playerController = GetComponent<PlayerController>();
        _animator = GetComponentInChildren<Animator>();
    }

    public void OnAttack(InputAction.CallbackContext value)
    {
        if (canAttack)
        {
            _playerController.Dash(5f);
            canAttack = false;
            _animator.SetTrigger("attack");
        }
    }
}
