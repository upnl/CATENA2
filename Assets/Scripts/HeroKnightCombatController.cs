using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class HeroKnightCombatController : CombatController
{
    public LayerMask groundLayer;
    private int _normalAttackNum;
    

    private void LateUpdate()
    {
        if (_normalAttackNum != 0 && !Animator.GetCurrentAnimatorStateInfo(0).IsTag("attack")) _normalAttackNum = 0;
    }

    public void OnAttack(InputAction.CallbackContext value)
    {
        if (canAttack && value.started)
        {
            CurrentAttackIndex = 0;
            CurrentAttackNumber = _normalAttackNum++;
            
            PlayerController.UpdateGfxDirection();
            Animator.SetTrigger("attack");
            UnableCanAttacks();
            attackCheckElapsedTime = 0.1f;

            PlayerController.UpdateCanVariables();
        }
    }

    public void OnSkill0(InputAction.CallbackContext value)
    {
        if (canAirAttack && value.started && !Animator.GetCurrentAnimatorStateInfo(0).IsTag("attack"))
        {
            CurrentAttackIndex = 1;
            CurrentAttackNumber = 0;
            
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
            CurrentAttackIndex = 2;
            CurrentAttackNumber = _normalAttackNum++;
            
            PlayerController.UpdateGfxDirection();
            StartCoroutine(Skill1());
            UnableCanAttacks();
            attackCheckElapsedTime = 0.1f;

            PlayerController.UpdateCanVariables();
        }
    }
    
    public void OnSkill2(InputAction.CallbackContext value)
    {
        if (canAttack && value.started)
        {
            CurrentAttackIndex = 3;
            CurrentAttackNumber = 0;
            
            PlayerController.UpdateGfxDirection();
            StartCoroutine(Skill2());
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
    
    private IEnumerator Skill2()
    {
        Animator.SetTrigger("skill2");

        bool isHit = false;
        PlayerController.superArmor = true;

        void CheckIsHit(object sender, EventArgs eventArgs)
        {
            isHit = true;
            Animator.SetTrigger("skill2_guard");
            PlayerController.OnhitEvent -= CheckIsHit;
            PlayerController.superArmor = false;
        }

        PlayerController.OnhitEvent += CheckIsHit;

        yield return new WaitForSeconds(3f);

        PlayerController.OnhitEvent -= CheckIsHit;
        PlayerController.superArmor = false;
        if (!isHit) Animator.SetTrigger("skill2_nothing");
    }

    protected override void UpdateCanAttack()
    {
        base.UpdateCanAttack();
    }
}
