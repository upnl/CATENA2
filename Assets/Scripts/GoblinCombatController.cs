using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoblinCombatController : MonsterCombatController
{
    public override void OnAttack(int attackIndex)
    {
        Attack0();
    }
    
    public void Attack0()
    {
        if (canAttack)
        {
            MonsterController.UpdateGfxDirection();
            Animator.SetTrigger("attack");
            canAttack = false;
            attackCheckElapsedTime = 0.1f;

            MonsterController.UpdateCanVariables();
        }
    }
}
