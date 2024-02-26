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
            PlayerController.UpdateGfxDirection();
            Animator.SetTrigger("attack");
            PlayerController.Dash(5f);
            canAttack = false;
            attackCheckElapsedTime = 0.1f;

            PlayerController.UpdateCanVariables();
        }
    }

    public void OnSkill0(InputAction.CallbackContext value)
    {
        if (canAttack && value.started)
        {
            PlayerController.UpdateGfxDirection();
            StartCoroutine(Skill0());
            canAttack = false;
            attackCheckElapsedTime = 0.1f;

            PlayerController.UpdateCanVariables();
        }
    }

    private IEnumerator Skill0()
    {
        Animator.SetTrigger("skill0");
        PlayerController.Jump(10f);
        PlayerController.Dash(10f);

        yield return new WaitForSeconds(0.3f);

        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 10f, groundLayer);

        PlayerController.Dash(1f);
        
        transform.position = hit.point;
        Animator.SetTrigger("skill0_land");
    }

    protected override void UpdateCanAttack()
    {
        base.UpdateCanAttack();
    }
}
