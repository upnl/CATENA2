using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class HeroKnightCombatController : CombatController
{
    
    public LayerMask groundLayer;
    public void OnAttack(InputAction.CallbackContext value)
    {
        if (canAttack && value.started)
        {
            currentAttackIndex = 0;
            currentAttackNumber = 0;
            
            PlayerController.UpdateGfxDirection();
            Animator.SetTrigger("attack");
            UnableCanAttacks();
            attackCheckElapsedTime = 0.1f;

            PlayerController.UpdateCanVariables();
        }
    }

    public void OnSkill0(InputAction.CallbackContext value)
    {
        if (canAirAttack && value.started)
        {
            currentAttackIndex = 1;
            currentAttackNumber = 0;
            
            PlayerController.UpdateGfxDirection();
            StartCoroutine(Skill0());
            UnableCanAttacks();
            attackCheckElapsedTime = 0.1f;

            PlayerController.UpdateCanVariables();
        }
    }
    
    public void OnSkill1(InputAction.CallbackContext value)
    {
        if (canAttack && value.started)
        {
            currentAttackIndex = 2;
            currentAttackNumber = 0;
            
            PlayerController.UpdateGfxDirection();
            StartCoroutine(Skill1());
            UnableCanAttacks();
            attackCheckElapsedTime = 0.1f;

            PlayerController.UpdateCanVariables();
        }
    }

    private IEnumerator Skill0()
    {
        // skill logic;
        Animator.SetTrigger("skill0");
        PlayerController.Jump(10f);
        PlayerController.Dash(10f);

        yield return new WaitForSeconds(0.3f);

        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 10f, groundLayer);

        Dash(1);
        
        transform.position = hit.point;
        Animator.SetTrigger("skill0_land");
    }
    
    private IEnumerator Skill1()
    {
        Animator.SetTrigger("attack");

        RaycastHit2D hit = Physics2D.Raycast(transform.position, 
            Vector2.right * (PlayerController.isFacingRight? 1 : -1), 10f, monsterLayer);

        if (hit)
        {
            if (PlayerController.isFacingRight)
            {
                transform.position = hit.point + Vector2.left;
            }
            else
            {
                transform.position = hit.point + Vector2.right;
            }
            
            hit.transform.GetComponent<CreatureController>().StopHitKnockBack();
        }

        yield return null;

        Dash(2);
    }

    protected override void UpdateCanAttack()
    {
        base.UpdateCanAttack();
    }
}
