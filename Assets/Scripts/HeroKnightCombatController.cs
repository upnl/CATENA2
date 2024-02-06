using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class HeroKnightCombatController : CombatController
{
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

    protected override void UpdateCanAttack()
    {
        base.UpdateCanAttack();
    }
}
