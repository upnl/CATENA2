using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class PlayerController : CreatureController
{
    // help variables to check player's state; 
    [Header("- help to check player's state")] 
    public float rollingElapsedTime;
    public float rollingDoneCheckTime;
    
    // check if player is able to do;
    [Header("- can****")]
    public bool canRoll;

    #region Event functions

    /*
     * Update Animation States; which contains direction of playerGFX;
     * Update can**** variables;
     * -- Update player's state variables;
     * -- Update State;
     */
    protected override void Update()
    {
        isRolling = Animator.GetCurrentAnimatorStateInfo(0).IsTag("rolling");
        isAttacking = Animator.GetCurrentAnimatorStateInfo(0).IsTag("attack");
        
        UpdateAnimationParameters();
        if (canMove) UpdateGfxDirection();
        UpdateCanVariables();
        UpdateHitState();
        CheckTimeElapsed(ref jumpingElapsedTime, ref isJumping);
        CheckTimeElapsed(ref stunTimeElapsed, ref isStun);
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
            Jump(jumpPower);
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
            Animator.SetTrigger("roll");
            
            UpdateCanVariables();
        }
    }
    
    
    #endregion

    #region update functions

    public override void UpdateCanVariables()
    {
        base.UpdateCanVariables();
        UpdateCanRoll();
    }

    protected override bool ConditionForUpdateCanVariable()
    {
        return isAttacking || isHit || isRolling;
    }
    
    private void UpdateCanRoll()
    {
        if (canRoll)
        {
            if (!isGrounded || isAttacking || isHit) canRoll = false;
            if (isRolling) canRoll = false;
        } else if (isGrounded && !isRolling && !isAttacking && !isHit) canRoll = true;
    }
    
    
    #endregion
    
    #region public methods
    
    #endregion
}
